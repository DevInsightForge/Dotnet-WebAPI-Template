# AGENTS Guide for DevInsightForge

This file defines operating rules for contributors and AI coding agents working on DevInsightForge.

## Scope

- Applies to the full DevInsightForge solution.
- Treat this as authoritative for architecture boundaries, coding patterns, and delivery quality.
- If business requirements conflict with these rules, follow requirements and update this guide in the same change.

## Solution Structure

Project responsibilities:

- `DevInsightForge.Domain`
  - Entities and core domain behavior only.
  - No infrastructure concerns and no EF-specific logic.
- `DevInsightForge.Application`
  - Use cases (commands/queries + handlers), DTOs, validation, result contracts, interfaces.
- `DevInsightForge.Persistence`
  - EF Core `DbContext`, entity configurations, migrations, repository/unit-of-work implementations.
- `DevInsightForge.Infrastructure`
  - Technical services (JWT token generation, password hashing, integrations).
- `DevInsightForge.WebAPI`
  - Composition root, middleware, auth, controllers, API response mapping.

## Dependency Direction (Must Follow)

- `WebAPI` -> `Application`, `Persistence`, `Infrastructure`
- `Infrastructure` -> `Application`
- `Persistence` -> `Application`, `Domain`
- `Application` -> `Domain`
- `Domain` -> (none)

Do not introduce reverse or circular dependencies.

## Mandatory Development Rules

- Keep controllers thin: no business rules, no direct data access.
- Use mediator request/handler pattern for use cases.
- Use `Result` / `Result<T>` from Application for operation outcomes.
- Map results to HTTP through `ToApiResponse`.
- Use FluentValidation for input validation.
- Keep write operations transactional with `IUnitOfWork.WithTransaction`.
- Respect `CancellationToken` across async paths.
- Do not reference `DbContext` outside Persistence.
- Do not leak infrastructure types into Domain.
- Register EF Core interceptors explicitly in DI before attaching them to `DbContext` options.

## Entity and Data Rules

- Entities use encapsulation (`private set`, methods/factory methods for mutation).
- IDs use GUID v7 by default (`Guid.CreateVersion7()`).
- Soft-delete is enabled through `IsDeleted` and global query filters.
- For soft-deletable entities, do not enforce uniqueness at database level (no unique constraints/indexes); enforce uniqueness in Application feature logic instead.
- Auditing is handled by `BaseAuditableEntity` + save interceptor.
- Use `BaseEntity` for identity/account tables unless full audit lineage is strictly required.
- For entities inheriting `BaseAuditableEntity`, keep audit FKs required and ensure create flows provide an actor context.
- Update EF configurations and migrations whenever the entity model changes.

## Validation and Error Rules

- Validation failures should return `ErrorType.Validation` with detailed field errors.
- Avoid exceptions for expected validation/business failures; return `Result.Failure(...)`.
- Unhandled exceptions are for unexpected runtime faults and are translated by global exception handler.
- Keep error codes stable (for example `auth.unauthorized`, `validation_error`) for client compatibility.

## Authentication and Authorization Rules

- API endpoints are authenticated by default.
- Use `[AllowAnonymous]` explicitly and sparingly.
- User identity comes from JWT `ClaimTypes.Sid`.
- Keep token claim schema consistent unless intentionally versioning auth contracts.

## Coding Guidelines

- Prefer explicit, intention-revealing names.
- Keep methods small and side effects obvious.
- Use async end-to-end for I/O paths.
- Keep comments minimal and meaningful; remove stale comments.
- Follow existing namespace conventions (including current `Abstructions` naming) unless performing deliberate, solution-wide cleanup.

## Feature Implementation Workflow (Agent Checklist)

1. Define or update domain behavior if model changes are required.
2. Add or extend DTOs and validators in Application.
3. Implement command/query + handler in Application.
4. Extend repository abstractions in Application only when required.
5. Implement persistence details in Persistence.
6. Register DI services for new components.
7. Add or extend WebAPI endpoints and response metadata.
8. Add or update migration for schema changes.
9. Build and run tests.
10. Verify response contracts and status codes remain consistent.

## Migrations and Database

Use the solution root as working directory.

- Never edit migration files manually; always create/update/remove migrations using EF CLI commands.

```bash
dotnet ef migrations add <MigrationName> --project DevInsightForge.Persistence --startup-project DevInsightForge.WebAPI
```

```bash
dotnet ef database update --project DevInsightForge.Persistence --startup-project DevInsightForge.WebAPI
```

## Quality Gate Before Merging

- Solution builds successfully.
- No layer boundary violations introduced.
- New endpoints return standardized `ApiResponse` shape.
- Validation and authorization behavior are covered.
- Migration files are included for schema changes.
- No secrets or environment-specific credentials committed.

## Out-of-Scope Changes Without Explicit Request

- Do not rename projects or namespaces broadly.
- Do not replace core patterns (mediator, result mapping, repository/UoW) in unrelated tasks.
- Do not perform sweeping refactors while implementing feature-only requests.

## If Rules Conflict

- Prioritize explicit user or business requirements.
- Document the exception in PR/commit notes.
- Update this guide so future agents follow the new standard.
