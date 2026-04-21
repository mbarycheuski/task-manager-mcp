# Claude Instructions for Task Manager MCP

## Project Documentation

- [docs/project-context.md](docs/project-context.md) — business requirements, tech stack, security model
- [docs/architecture.md](docs/architecture.md) — project structure, API spec, MCP server spec

## Running the App

Use `run.ps1` from the repo root (requires Docker and a `.env` file — copy `.env.example` to get started):

```powershell
.\run.ps1          # start services
.\run.ps1 -Build   # rebuild images before starting
.\run.ps1 -Down    # stop and remove containers
```

## Code Conventions

### C# API (`/src/api`)

- Use controller-based API (not minimal API)
- 3-layer architecture: Controller (DTO) → Service (DTO) → Repository (Entity) → DbContext
- Controllers only handle HTTP concerns — no business logic
- Services contain all business logic and DTO ↔ Entity mapping
- Keep DTOs separate from entity models — never expose entities directly
- Use FluentValidation for input validation (not data annotations); validate at the controller layer, not in services
- Never put secrets in `appsettings.json` — use user secrets or environment variables
- Use Guid for entity primary keys
- Use primary constructors for all classes
- Initialize required `string` properties on EF Core entities with `null!` (not `string.Empty`) — EF Core guarantees these are set on materialization
- Use `record` for all DTOs
- In enums, only set an explicit value on the first member
- Place `using` directives before the `namespace` declaration (not inside it)
- Name parameters after their type in camelCase (e.g. `CreateTaskRequest createTaskRequest`, `CancellationToken cancellationToken`); short conventional names like `id`, `entity`, `status` are fine
- Always validate input arguments at the start of every public method (including static and extension methods): use `ArgumentException.ThrowIfNullOrWhiteSpace` for `string` parameters, `ArgumentNullException.ThrowIfNull` for other reference types, and `ArgumentOutOfRangeException.ThrowIfEqual` for invalid values (e.g. `Guid.Empty`); value types and nullable value types where `null` is a valid input do not need a null check
- Always add a blank line before `return` statements
- Use `TimeProvider` (injected via DI) instead of `DateTime.UtcNow` directly
- Use `SingleOrDefaultAsync` (not `FirstOrDefaultAsync`) when querying by unique identifier — signals intent and throws on unexpected duplicates
- Never inject `DbContext` directly outside of Repository and Seeder classes — all other classes must go through a repository interface

### Python MCP Server (`/src/mcp`)

- Use FastMCP framework
- All tools must call the C# API over HTTP (not access the DB directly)
- Tool descriptions must be clear and AI-friendly
- Read API key from environment variables or `.env` file

## When Making Changes

1. **Read first** — read the relevant section of `docs/architecture.md` and `docs/project-context.md` before writing any code.
2. **Find existing patterns** — locate similar code in the codebase and follow the same structure.
3. **Implement**
   - *C# API*: make the change across all affected layers (Controller → Service → Repository).
   - *Python MCP*: add or update the tool, ensuring it calls the C# API over HTTP.
4. **Update docs** — if the change adds or modifies an endpoint or tool, update `docs/architecture.md`.
5. **Verify**
   - *C# API*: run the following from `src/api/TaskManager.Api` and confirm both pass before reporting done:
     ```bash
     dotnet build TaskManager.Api.csproj
     dotnet test TaskManager.Api.csproj
     ```
6. **Review** — run the `code-reviewer` agent after every change to verify consistency with project patterns, coding standards, and architectural conventions.
