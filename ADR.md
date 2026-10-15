# Architecture Decision Records

## ADR-001 — Architecture & Dependency Direction

### Context

Learniverse is split into Domain, Application, Persistence, Infrastructure, and API.

The intended dependency direction is:

Application → Domain

Persistence → Application
Persistence → Domain

Infrastructure → Application
Infrastructure → Domain
Infrastructure → Persistence

API → Application
API → Infrastructure

`API` currently references `Persistence` for composition/DI setup. This is a project reference, not an architectural dependency rule.

Application must not reference Persistence or depend directly on `AppDbContext`.

### Decision

Keep the current Clean Architecture boundaries.

- Domain contains entities and domain rules.
- Application contains use cases, handlers, validators, DTOs, and abstractions.
- Persistence contains EF Core and database-related implementations.
- Infrastructure contains technical implementations such as Identity and authentication-related services.
- API contains HTTP concerns and application composition.

Application code should depend on abstractions, not Persistence or EF Core implementations.

### Consequences

This keeps the Application layer independent from EF Core and concrete infrastructure implementations.

---

## ADR-002 — Application Flow & Persistence Abstractions

### Context

Application use cases use CQRS and MediatR. Persistence access is hidden behind repository abstractions.

### Decision

Use Commands and Queries through MediatR.

Write operations follow:

Create / Update / Delete

→ Repository  
→ Aggregate / Domain rules  
→ IUnitOfWork

Read operations follow:

Get / GetAll / Search

→ Repository abstraction

Application handlers must not use `AppDbContext` directly.

`IUnitOfWork` exposes:

- `SaveChangesAsync`
- `BeginTransactionAsync`
- `CommitTransactionAsync`
- `RollbackTransactionAsync`

`AppDbContext` implements `IUnitOfWork`.

### Consequences

Handlers stay independent from EF Core and persistence implementation details.

---

## ADR-003 — DDD Aggregate Boundaries

### Context

Course content is structured as:

Course → Sections → Lessons

### Decision

`Course` is the Aggregate Root.

`Section` belongs to `Course`, and `Lesson` belongs to `Section`.

Mutations should go through the appropriate aggregate behavior instead of treating child entities as independent aggregates.

This applies to domain mutations. Read operations do not have to traverse the aggregate hierarchy.

### Consequences

Domain rules stay within the aggregate that owns them.

---

## ADR-004 — Persistence Policies

### Context

The system needs consistent behavior for deletion and auditing.

### Decision

Use soft delete for:

- `Course`
- `Section`
- `Lesson`
- `Category`

EF Core global query filters exclude soft-deleted records from normal queries.

Soft-deleted records should only be accessed explicitly when a use case requires it.

Audit timestamps are handled centrally in `AppDbContext.SaveChangesAsync`.

For new entities:

`CreatedAtUtc` is set automatically.

For modified entities:

`CreatedAtUtc` is preserved and `UpdatedAtUtc` is updated.

Handlers should not manage these timestamps manually.

### Consequences

Deletion and audit behavior stay consistent across the application.

---

## ADR-005 — Identity Architecture, Registration & Authentication

### Context

Learniverse uses ASP.NET Core Identity for user and role management. The API uses JWT Bearer authentication.

### Decision

`ApplicationUser` extends `IdentityUser`.

`AppDbContext` inherits from:

`IdentityDbContext<ApplicationUser>`

Application use cases access Identity operations through the application-level `IIdentityService` abstraction. Its concrete implementation is handled in Infrastructure.

The current roles are:

- `Student`
- `Instructor`
- `Admin`

Self-registration creates a user and assigns the `Student` role. User creation and initial role assignment are handled in one transaction.

### Current Implementation

Login accepts an email address or username and uses ASP.NET Core Identity sign-in checks, including lockout handling.

Identity currently requires unique email addresses. Passwords must be at least 8 characters and contain an uppercase letter, a lowercase letter, a digit, and a non-alphanumeric character. Lockout is enabled for new users after 5 failed attempts for 15 minutes.

The API issues JWT Bearer access tokens containing the user ID as the subject and the user's roles. Tokens are signed with HMAC-SHA256 and use configured issuer, audience, and expiration values.

The API validates the token signature, issuer, audience, and lifetime. JWT options are validated at application startup.

Endpoint access uses authorization attributes, including role restrictions, together with application-level access checks where needed.

### Consequences

Application use cases remain independent from ASP.NET Core Identity implementation types.

Authentication and role claims are available to API authorization and current-user services.

### Not Implemented

Refresh-token support is not currently implemented.