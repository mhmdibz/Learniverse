# Architecture Decision Records

## ADR-001 — Architecture & Dependency Direction

**Status:** Accepted

### Context

Learniverse is split into `Domain`, `Application`, `Persistence`, `Infrastructure`, and `API`.

The intended dependency direction is:

Application → Domain

Persistence → Application
Persistence → Domain

Infrastructure → Application
Infrastructure → Domain
Infrastructure → Persistence

API → Application
API → Infrastructure

The API is the composition root and currently also references `Persistence` to register its services. This implementation reference does not allow `Application` to depend on `Persistence`.

### Decision

Keep the current Clean Architecture boundaries.

- `Domain` contains entities and domain rules.
- `Application` contains use cases, handlers, validators, DTOs, and abstractions.
- `Persistence` contains EF Core and database-related implementations.
- `Infrastructure` contains technical implementations such as Identity and authentication-related services.
- `API` contains HTTP concerns and application composition.

Application code must depend on abstractions rather than concrete Persistence or EF Core implementations. `Application` must not reference `Persistence` or depend directly on `AppDbContext`.

### Consequences

This keeps Application independent from EF Core and concrete infrastructure implementations. The API may reference implementation projects as needed to compose the application at startup.

---

## ADR-002 — Application Flow & Persistence Abstractions

**Status:** Accepted

### Context

Application use cases use CQRS and MediatR. Database access is hidden behind repository abstractions.

### Decision

Use Commands and Queries through MediatR.

Write operations follow:

Command Handler → Repository → Aggregate / Domain behavior → IUnitOfWork

Read operations follow:

Query Handler → Repository abstraction → DTO projection in the query

Rules:

- Application handlers must not use `AppDbContext` directly.
- Repositories must not expose `IQueryable`.
- Query projections should be performed in the Repository before materialization, and queries should return DTOs rather than tracked domain entities.
- Queries do not use `IUnitOfWork`.
- Commands use `IUnitOfWork` to save changes and, when needed, manage transactions.

`IUnitOfWork` exposes:

- `SaveChangesAsync`
- `BeginTransactionAsync`
- `CommitTransactionAsync`
- `RollbackTransactionAsync`

`AppDbContext` implements `IUnitOfWork`.

### Known Deviation

Some existing query handlers load entities and map them to response DTOs in Application. This is a known deviation from the query projection rule. Do not refactor these handlers as part of unrelated work; discuss the change first.

### Consequences

Handlers remain independent from EF Core and persistence implementation details. Query projection rules also make it clear where database-side filtering and mapping belong.

---

## ADR-003 — DDD Aggregate Boundaries

**Status:** Accepted

### Context

Course content is structured as:

Course → Section → Lesson

`Category` and `Enrollment` are separate aggregates.

### Decision

- `Course` is the aggregate root for course structure.
- `Section` belongs to a `Course`.
- `Lesson` belongs to a `Section`.
- Mutations must use the relevant domain behavior and preserve the owning aggregate's rules; child entities are not treated as unrelated aggregates.
- The domain model intentionally has no parent navigation from `Section` to `Course` or from `Lesson` to `Section`. Queries that need parent data use explicit joins in Repository implementations.
- Child construction is controlled by the owning domain behavior. Child constructors are `internal` where required by the current model.
- Repositories are specific to the project's aggregates; do not introduce a generic repository.

This decision applies to domain mutations. Read operations do not have to traverse the aggregate hierarchy.

### Consequences

Domain rules stay near the entities that own them, while query implementations can use explicit joins without adding parent navigations that weaken the chosen boundaries.

---

## ADR-004 — Persistence, Soft Delete & Auditing

**Status:** Accepted

### Context

The system needs consistent behavior for deletion, query visibility, and auditing.

### Decision

Use soft delete for:

- `Course`
- `Section`
- `Lesson`
- `Category`

`Enrollment` does not use soft delete; its status represents its lifecycle.

EF Core global query filters exclude soft-deleted records from normal queries. Soft-deleted records should be accessed explicitly only when a use case requires it.

Soft deletion follows the domain's cascade rules:

- Deleting a course soft-deletes its sections and lessons.
- Deleting a section also soft-deletes its lessons.

Unique indexes on soft-deletable entities must account for soft-deleted rows, using filtered indexes where appropriate.

Global query filters apply to each `DbSet`, including queries involving joins. Code that reaches a parent through a child must handle the parent being unavailable, including when it has been soft-deleted.

Audit timestamps are handled centrally in `AppDbContext.SaveChangesAsync`:

- New entities receive `CreatedAtUtc`.
- Modified entities retain `CreatedAtUtc` and receive an updated `UpdatedAtUtc`.
- Handlers must not set these timestamps manually.

Course status `Archived` is distinct from soft deletion; its access behavior is documented in ADR-006.

### Consequences

Deletion and auditing behavior stay consistent. Query filters must be considered when joining related data, and soft deletion must not be confused with archiving.

---

## ADR-005 — Identity, Registration & Authentication

### Context

Learniverse uses ASP.NET Core Identity for user and role management. The API uses JWT Bearer authentication.

### Decision

`ApplicationUser` extends `IdentityUser`.

`AppDbContext` inherits from:

`IdentityDbContext<ApplicationUser>`

Application use cases access Identity operations through the application-level `IIdentityService` abstraction. Its concrete implementation is handled in Infrastructure.

Self-registration creates a user and assigns the `Student` role. User creation and initial role assignment are handled in one transaction.

### Current Implementation

The current roles are:

- `Student`
- `Instructor`
- `Admin`

Login accepts an email address or username and uses ASP.NET Core Identity sign-in checks, including lockout handling.

Identity currently requires unique email addresses. Passwords must be at least 8 characters and contain an uppercase letter, a lowercase letter, a digit, and a non-alphanumeric character. Lockout is enabled for new users after 5 failed attempts for 15 minutes.

The API issues JWT Bearer access tokens containing the user ID as the subject and the user's roles. Tokens are signed with HMAC-SHA256 and use configured issuer, audience, and expiration values.

The API validates the token signature, issuer, audience, and lifetime. JWT options are validated at application startup.

### Consequences

Application use cases remain independent from ASP.NET Core Identity implementation types.

Authentication and role claims are available to API authorization and current-user services.

### Not Implemented

Refresh-token support is not currently implemented.

---

## ADR-006 — Course Visibility & Content Access

### Context

Course status, ownership, and enrollment determine who can view course metadata, sections, and lessons. Course visibility is separate from course modification authorization.

### Decision

A course owner is identified by matching the current user's ID with `Course.InstructorId`. The owner does not need the `Instructor` role for course visibility.

For course metadata and sections:

| Course status | Who can view |
|---|---|
| `Published` | Everyone, including anonymous users |
| `Archived` | Admin, owner, or a student with an `Active` or `Completed` enrollment |
| `Draft` | Admin or owner |

Full course lesson endpoints require authentication.

| Course status | Who can access all lessons |
|---|---|
| `Published` | Admin, owner, or a student with an `Active` or `Completed` enrollment |
| `Archived` | Admin, owner, or a student with an `Active` or `Completed` enrollment |
| `Draft` | Admin or owner; enrollment does not grant access |

For a `Published` course, an authenticated user who can view the course but is not enrolled receives `403 Forbidden` when requesting all course lessons.

For an `Archived` or `Draft` course, authenticated users who are not allowed to view its lessons receive `404 Not Found`.

Preview lesson metadata remains public for preview lessons in `Published` courses through the existing public lesson endpoints. Preview access does not make the rest of a course's lessons public.

Course modification authorization is separate from visibility. Modification endpoints require the `Instructor` or `Admin` role and use `ICourseAuthorizationService` to check whether the caller is an Admin or the course owner.

### Consequences

Students with `Active` or `Completed` enrollments retain access to `Archived` courses. `Draft` courses and other hidden courses remain protected. Course visibility and course modification remain separate authorization decisions.

---

## Pending Decisions — Enrollment & Payment

These items must be discussed before documenting them as accepted architecture or product decisions:

- An external payment provider is planned for the Enrollment flow, but no provider has been selected.
- Enrollment creation and the payment lifecycle are not complete. Payment states, retries, cancellations, refunds, and provider-specific behavior remain undecided.
- Payment confirmation must be handled by the server; webhook verification and duplicate-event handling still need to be decided. A browser redirect alone must not activate an enrollment.
- The current unique index on `(StudentId, CourseId)` affects re-enrollment after cancellation; the desired behavior is undecided.
- The behavior of cancelling a `Completed` enrollment needs confirmation.
- Whether courses can be free, paid, or both remains a product decision.
- The effect of soft-deleting a course with active or paid enrollments needs a business decision. Soft deletion is separate from archiving.