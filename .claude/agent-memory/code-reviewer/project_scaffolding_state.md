---
name: Project implementation state
description: Current implementation state of both projects as of the TaskManager.Mcp full implementation changeset
type: project
---

As of the TaskManager.Mcp full-implementation changeset (2026-04-25), both projects are substantially implemented.

**TaskManager.Api** — fully implemented: Auth, Controllers, Services, Repositories, Mappers, Validators, Exceptions, EF Core Data layer with migrations and seeder. See prior memory content for detailed layer inventory.

**TaskManager.Mcp** — fully implemented:
- **Tools** — `TaskTools` (get_all_tasks, get_task, add_task, update_task, delete_task)
- **Resources** — `TaskResources` (tasks://all, tasks://completed, tasks://in-progress, tasks://today, tasks://{id})
- **Services** — `ITaskService` / `TaskService`
- **Collaborators** — `ITaskApiCollaborator` / `TaskApiCollaborator` (typed HttpClient); `TaskApiConstants` (Endpoints + Headers)
- **Collaborators/Dto** — `TaskItemDto`, `CreateTaskRequestDto`, `UpdateTaskRequestDto`, `TaskItemStatusDto`, `TaskPriorityDto`
- **Inputs** — `CreateTaskInput`, `UpdateTaskInput` (record types)
- **Outputs** — `TaskItem`, `TaskItemStatus`, `TaskPriority`
- **Mappers** — `TaskItemMappingExtensions` (Dto → Output, Input → Dto)
- **Common** — `ITimeService` / `TimeService` (timezone-aware today calculation)
- **Settings** — `McpSettings` (ApiBaseUrl, ApiKey, Timezone)
- **Utilities** — `ApiCallHelper`, `MediaTypes`, `ValidationConstants`, `IOutputSerializer` / `JsonOutputSerializer`

**Why:** Tracks what is present so future reviews can evaluate all layers against full convention set.
**How to apply:** Do not treat any layer as scaffolding-only. Review all layers against the full convention set documented in CLAUDE.md.
