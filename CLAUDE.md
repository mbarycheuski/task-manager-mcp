# Claude Instructions for Task Manager MCP

## Project Documentation

- [docs/project-context.md](docs/project-context.md) — business requirements, tech stack, security model
- [docs/architecture.md](docs/architecture.md) — project structure, API spec, MCP server spec

## Code Conventions

### C# API (`/src/api`)

- Use controller-based API (not minimal API)
- 3-layer architecture: Controller (DTO) → Service (DTO) → Repository (Entity) → DbContext
- Controllers only handle HTTP concerns — no business logic
- Services contain all business logic and DTO ↔ Entity mapping
- Use FluentValidation for input validation (not data annotations)
- Never put secrets in `appsettings.json` — use user secrets or environment variables
- Use Guid for entity primary keys

### Python MCP Server (`/src/mcp`)

- Use FastMCP framework
- All tools must call the C# API over HTTP (not access the DB directly)
- Tool descriptions must be clear and AI-friendly
- Read API key from environment variables or `.env` file

## When Making Changes

- Read the relevant architecture doc section before implementing
- Follow existing patterns in the codebase
- Keep DTOs separate from entity models — never expose entities directly
- Validate at the controller layer, not in services
- When adding new endpoints, update the architecture doc
