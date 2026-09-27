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

## ADR-005 — Identity Architecture & Registration

### Context

Learniverse uses ASP.NET Core Identity for user and role management.

### Decision

`ApplicationUser` extends `IdentityUser`.

`AppDbContext` inherits from:

`IdentityDbContext<ApplicationUser>`

Application code does not depend directly on ASP.NET Core Identity implementation types.

Identity operations are exposed to Application through an application-level abstraction, with the concrete implementation handled outside the Application layer.

Current roles:

- `Student`
- `Instructor`
- `Admin`

Self-registration creates a user and assigns the `Student` role.

User creation and initial role assignment are handled as one transaction.

### Consequences

Application remains independent from the Identity implementation while Persistence/Infrastructure can use ASP.NET Core Identity internally.

### Pending

Login and authentication design is intentionally not covered here.

The following are still pending:

- Login identifier
- Password verification
- Sign-in/lockout behavior
- JWT generation
- JWT claims
- Token lifetime
- Issuer / Audience / signing configuration
- Refresh tokens
- Authorization policies