---
name: Project implementation state
description: Current implementation state of both projects as of the exception-middleware changeset (2026-04-26)
type: project
---

As of the exception-middleware changeset (2026-04-26), both projects are substantially implemented.

**TaskManager.Api** — fully implemented: Auth, Controllers, Services, Repositories, Mappers, Validators, Exceptions (`ApiException` base → `NotFoundException`, `BusinessException`), EF Core Data layer with migrations and seeder. Global exception handling via `IExceptionHandler` (`ApiExceptionHandler`).

**TaskManager.Mcp** — fully implemented:
- **Tools** — `TaskTools` (get_all_tasks, get_task, add_task, update_task, delete_task)
- **Resources** — `TaskResources` (tasks://all, tasks://completed, tasks://in-progress, tasks://today, tasks://{id})
- **Services** — `ITaskService` / `TaskService`; `ApiErrorHandler` (internal static helper, lives in `Services/`)
- **Collaborators** — `ITaskApiCollaborator` / `TaskApiCollaborator` (typed HttpClient); `TaskApiConstants` (Endpoints + Headers)
- **Collaborators/Dto** — `TaskItemDto`, `CreateTaskRequestDto`, `UpdateTaskRequestDto`, `TaskItemStatusDto`, `TaskPriorityDto`
- **Inputs** — `CreateTaskInput`, `UpdateTaskInput` (record types)
- **Outputs** — `TaskItem`, `TaskItemStatus`, `TaskPriority`
- **Mappers** — `TaskItemMappingExtensions` (Dto → Output, Input → Dto)
- **Common** — `ITimeService` / `TimeService` (timezone-aware today calculation using injected `TimeProvider`)
- **Settings** — `McpSettings` (ApiBaseUrl, ApiKey, Timezone)
- **Utilities** — `MediaTypes`, `ValidationConstants`, `IOutputSerializer` / `JsonOutputSerializer`
- **Exceptions** — `AppException` (base), `NotFoundException`, `ValidationException`
- **Middleware** — `ExceptionMiddleware` (maps `AppException` subtypes to `McpProtocolException`)
- **Prompts** — directory exists but is EMPTY; `daily-plan` and `prioritize-tasks` documented but not implemented

**Layer naming (canonical):**
- `Inputs/` = MCP input parameter records
- `Outputs/` = domain/output types for Service and LLM
- `Exceptions/` = `AppException` hierarchy (MCP-side)
- `Middleware/` = ASP.NET Core middleware (ExceptionMiddleware)

**Known open issues (from 2026-04-26 review):**
- `AppException` base class is not sealed, but its name deviates from the Api-side convention (`ApiException` vs `AppException`). Not a blocker but worth noting.
- `ExceptionMiddleware` is not `sealed`; `AppException` base class is not `abstract` — both are minor convention gaps relative to the Api-side pattern.
- `NotFoundException` maps to `McpErrorCode.InternalError` instead of a more semantically accurate code (no `NotFound` exists in MCP error codes — this is a justified constraint).
- `ApiErrorHandler` lives in `Services/` (correct) but carries no interface — tested only by integration.

**Why:** Tracks what is present so future reviews can evaluate all layers against full convention set.
**How to apply:** Do not treat any layer as scaffolding-only. Review all layers against the full convention set documented in CLAUDE.md.
