# Claude Instructions for Task Manager MCP

## Project Documentation

- [docs/project-context.md](docs/project-context.md) — business requirements, tech stack, security model
- [docs/architecture.md](docs/architecture.md) — project structure, API spec, MCP server spec

## Running the App

Docker runs inside WSL. Use `run.ps1` from the repo root (requires a `.env` file — copy `.env.example` to get started):

```powershell
.\run.ps1              # start services (starts WSL Docker daemon if needed)
.\run.ps1 -Build       # rebuild images before starting
.\run.ps1 -Down        # stop and remove containers
.\run.ps1 -SyncCerts   # sync Windows CA certificates to WSL (run once after cert changes)
```

## Code Conventions

### General C#

- Never put secrets in `appsettings.json` — use user secrets or environment variables
- Use primary constructors for all classes
- Use `record` for all DTOs/contracts
- In enums, only set an explicit value on the first member
- Place `using` directives before the `namespace` declaration (not inside it)
- Name parameters after their type in camelCase (e.g. `CreateTaskRequest createTaskRequest`, `CancellationToken cancellationToken`); short conventional names like `id`, `entity`, `status` are fine
- Always validate input arguments at the start of every public method (including static and extension methods): use `ArgumentException.ThrowIfNullOrWhiteSpace` for `string` parameters, `ArgumentNullException.ThrowIfNull` for other reference types, and `ArgumentOutOfRangeException.ThrowIfEqual` for invalid values (e.g. `Guid.Empty`); value types and nullable value types where `null` is a valid input do not need a null check
- Always add a blank line before `return` statements
- Use `TimeProvider` (injected via DI) instead of `DateTime.UtcNow` directly

### TaskManager.Api (`/src/TaskManager.Api`)

- Use controller-based API (not minimal API)
- 3-layer architecture: Controller (Contract) → Service (Contract) → Repository (Entity) → DbContext
- Controllers only handle HTTP concerns — no business logic
- Services contain all business logic and Contract ↔ Entity mapping
- Keep contracts separate from entity models — never expose entities directly
- Use FluentValidation for input validation (not data annotations); validate at the controller layer, not in services
- Use Guid for entity primary keys
- Initialize required `string` properties on EF Core entities with `null!` (not `string.Empty`) — EF Core guarantees these are set on materialization
- Use `SingleOrDefaultAsync` (not `FirstOrDefaultAsync`) when querying by unique identifier — signals intent and throws on unexpected duplicates
- Never inject `DbContext` directly outside of Repository and Seeder classes — all other classes must go through a repository interface

### TaskManager.Mcp (`/src/TaskManager.Mcp`)

- Use `ModelContextProtocol.AspNetCore`; expose Streamable HTTP transport in stateless mode
- 3-layer architecture: Tool / Resource / Prompt → Service → Collaborator → HttpClient → TaskManager.Api
- Tools, resources, and prompts handle only MCP concerns — no HTTP calls or business logic inside them
- Services contain orchestration logic; no MCP types (`McpServer`, context objects) in services
- Collaborators are typed `HttpClient` wrappers; `X-Api-Key` is set once as a default request header at DI registration — never per-call
- Exception handling: throw `ValidationException` for input validation errors, `NotFoundException` for missing resources, `AppException` for API errors; an `AddCallToolFilter` registered in `Program.cs` catches these and converts to `McpProtocolException` with appropriate error codes; `ApiErrorHandler` wraps service calls and converts `HttpRequestException` to domain exceptions

## When Making Changes

1. **Read first** — read the relevant section of `docs/architecture.md` and `docs/project-context.md` before writing any code.
2. **Find existing patterns** — locate similar code in the codebase and follow the same structure.
3. **Implement**
   - *TaskManager.Api*: make the change across all affected layers (Controller → Service → Repository).
   - *TaskManager.Mcp*: add or update the tool/resource/prompt and propagate through Service → Collaborator.
4. **Update docs** — if the change adds or modifies an endpoint or tool, update `docs/architecture.md`.
5. **Verify**
   - Run the following from `src/TaskManager.Api` and confirm both pass before reporting done:
     ```bash
     dotnet build TaskManager.Api.csproj
     dotnet test TaskManager.Api.csproj
     ```
6. **Review** — run the `code-reviewer` agent after every change to verify consistency with project patterns, coding standards, and architectural conventions.
