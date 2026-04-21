---
name: Project current state
description: Current implementation state of the Task Manager API — all layers and components that exist as of the API key authentication changeset
type: project
---

As of the API key authentication changeset (2026-04-21), the following layers are fully implemented:

- **Controllers** — `TasksController` (all CRUD endpoints, `[Authorize]` applied)
- **Services** — `ITaskService` / `TaskService`
- **Repositories** — `ITaskRepository` / `TaskRepository`, `IApiKeyRepository` / `ApiKeyRepository`
- **Data** — `TaskDbContext`, `ApiKeySeeder`, entity models (`TaskItem`, `ApiKey`)
- **Auth** — `ApiKeyAuthenticationHandler`, `ApiKeyCacheService`, `IApiKeyCacheService`, `ApiKeyDefaults`, `ApiKeyHasher`, `IApiKeyHasher` (note: architecture.md places `ApiKeyHasher` and `ApiKeyCacheService` in `Services` folder, but code puts them in `Auth` folder)
- **Common/Utils** — `TimeService`, `ITimeService`
- **Contracts** — request/response record types in `/Contracts` namespace; contract enums in `/Contracts/Enums`; note types use names like `TaskItem`, `CreateTaskRequest` rather than `Contract` suffix (pre-existing pattern, not introduced by this changeset)
- **Validators** — FluentValidation validators wired up via `AddFluentValidationAutoValidation`
- **Settings** — `AppSettings` bound from `appsettings.json` (Timezone, ApiKeyCacheSlidingExpiration); uses `[Required]` data annotations for options validation (acceptable — this is .NET Options pattern, not controller input validation)
- **Exception handling** — `ApiExceptionHandler` (`IExceptionHandler`), `NotFoundException`, `BusinessException`

**Why:** Tracks what is actually present so future reviews don't flag missing layers as absent.
**How to apply:** Use as baseline when reviewing new files — don't flag "layer doesn't exist" for layers listed above.
