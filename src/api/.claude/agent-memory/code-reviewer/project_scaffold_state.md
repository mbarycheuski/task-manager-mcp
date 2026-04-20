---
name: Task CRUD implementation state
description: As of April 2026, Task CRUD is fully implemented across all layers; Auth layer (ApiKeyAuthenticationHandler, ApiKeyHasher, ApiKeyCacheService, ApiKeySeeder) is still pending.
type: project
---

As of the Task CRUD commit (April 2026), the following layers are now implemented in `src/api/TaskManager.Api`:

- `Contracts/` — `TaskItem` (response, lacks `Contract` suffix per convention), `CreateTaskRequest`, `UpdateTaskRequest`, `TaskQueryFilters`, contract enums (`TaskItemStatus`, `TaskPriority`)
- `Exceptions/` — `NotFoundException`, `BusinessException`, `ApiException` (base), `ApiExceptionHandler` (global `IExceptionHandler`)
- `Mappers/` — `TaskItemMappingExtensions` (static extension methods: `ToContract`, `ToDomain`)
- `Repositories/` — `ITaskRepository`, `TaskRepository` (with `GetByIdReadOnlyAsync` for read paths, `GetByIdAsync` for tracked update/delete paths)
- `Services/` — `ITaskService`, `TaskService`
- `Validators/` — `CreateTaskRequestValidator`, `UpdateTaskRequestValidator`, `TaskQueryFiltersValidator`
- `Controllers/` — `TasksController`
- `Common/` — `ITimeService`, `TimeService` (timezone-aware "today" date for due-date validation)
- `Settings/` — `AppSettings` (holds `Timezone` string)
- `Program.cs` — DI registrations for all layers + exception handling middleware

**Contract naming gap**: Contract response/request types use names like `TaskItem`, `CreateTaskRequest` — they lack the `Contract` suffix required by the project convention (`TaskItemContract`, `CreateTaskRequestContract`, etc.). This is a known pre-existing issue in the codebase, not flagged as new.

**Auth layer (ApiKeyAuthenticationHandler, ApiKeyHasher, ApiKeyCacheService, ApiKeySeeder) is still not implemented.** `UseAuthentication()` and `AddAuthentication()` are absent from `Program.cs` — this is a known pre-existing gap.

**Why:** Task CRUD was implemented as the first feature after scaffolding. Auth layer is a separate upcoming task.

**How to apply:** When reviewing future changes, the full CRUD stack exists and can be used as a pattern reference. The missing auth middleware and the contract-suffix gap are known pre-existing conditions.

**`TimeProvider` is registered as `TimeProvider.System` singleton in `Program.cs`.** `TaskService` injects `TimeProvider` directly for UTC timestamps (`UpdatedAt`, `CompletedAt`). `ITimeService` (wraps `TimeProvider` + timezone config) is injected into validators for due-date "today" comparisons. This split is intentional.

**Read-only repository pattern**: `ITaskRepository` exposes two get-by-id variants: `GetByIdAsync` (tracked, for `UpdateAsync`) and `GetByIdReadOnlyAsync` (AsNoTracking, for read-only `GetByIdAsync` in controller and delete path).
