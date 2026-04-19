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
- Use `record` for all DTOs
- In enums, only set an explicit value on the first member
- Place `using` directives before the `namespace` declaration (not inside it)

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
   - *C# API*: run the following from `src/api` and confirm both pass before reporting done:
     ```bash
     dotnet build
     dotnet test
     ```
6. **Review** — run the `code-reviewer` agent after every change to verify consistency with project patterns, coding standards, and architectural conventions.
