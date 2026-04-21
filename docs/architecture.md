# Task Manager MCP — Architecture

## Project Structure

```
/docs                        — project documentation
/src
  TaskManager.slnx           — solution file (both projects)
  /TaskManager.Api           — C# .NET 10 CRUD API
    /Auth                    — API key authentication handler, hasher, cache service
    /Common                  — shared services (TimeService)
    /Contracts               — request/response contract records
    /Contracts/Enums         — enums used in contracts
    /Controllers             — API controllers
    /Data                    — DbContext, migrations, seeders, EF configurations
    /Data/Configurations     — EF Core entity configurations
    /Data/Migrations         — EF Core migrations
    /Data/Models             — EF Core entity models
    /Data/Models/Enums       — enums used by entity models
    /Exceptions              — NotFoundException, BusinessException
    /Exceptions/Handlers     — global IExceptionHandler implementations
    /Mappers                 — entity ↔ contract mapping extensions
    /OpenApi                 — Swagger/OpenAPI customizations
    /Repositories            — repository interfaces and implementations
    /Services                — business logic layer
    /Settings                — strongly-typed configuration classes
    /Validators              — FluentValidation validators
    Dockerfile
  /TaskManager.Mcp           — C# .NET 10 MCP server (ModelContextProtocol.AspNetCore)
    /Tools                   — [McpServerToolType] / [McpServerTool]
    /Resources               — [McpServerResourceType] / [McpServerResource]
    /Prompts                 — [McpServerPromptType] / [McpServerPrompt]
    /Services                — orchestration layer (no MCP or HTTP concerns)
    /Collaborators           — typed HttpClient wrappers for TaskManager.Api
    /Contracts               — MCP-facing request/response records
    /Settings                — strongly-typed configuration (API base URL, API key)
    Dockerfile
docker-compose.yml           — api, db, mcp services
.env                         — secrets injected into containers (git-ignored)
```

---

## API (TaskManager.Api)

### Layer Flow

```
X-Api-Key header → ApiKeyAuthenticationHandler (in-memory cache)
    ↓
Controller  — HTTP concerns, FluentValidation auto-validates requests
    ↓
Service     — business logic, Contract ↔ Entity mapping via /Mappers
    ↓
Repository  — data access via EF Core
    ↓
DbContext
```

### Task Model

| Field       | Type      | Notes                                           |
|-------------|-----------|-------------------------------------------------|
| Id          | Guid      |                                                 |
| Title       | string    | Required, max 255 chars                         |
| Notes       | string?   | Max 4000 chars                                  |
| Priority    | enum?     | Low, Medium, High, Critical                     |
| Status      | enum      | None, InProgress, Completed                     |
| DueDate     | DateOnly? |                                                 |
| CreatedAt   | DateTime  | Server-set                                      |
| UpdatedAt   | DateTime  | Server-set                                      |
| CompletedAt | DateTime? | Set on transition to Completed                  |

### API Key Authentication

All endpoints require a valid API key in the `X-Api-Key` header via a custom ASP.NET Core `AuthenticationHandler`.

#### ApiKey Model

| Field      | Type     | Notes                                  |
|------------|----------|----------------------------------------|
| Id         | Guid     |                                        |
| ClientName | string   | e.g. "mcp-server"                      |
| KeyHash    | string   | HMAC-SHA256(key: salt, msg: rawApiKey) |
| Salt       | string   | Random per-key salt (base64)           |
| CreatedAt  | DateTime |                                        |
| IsActive   | bool     | Soft revocation (default: true)        |

#### Hashing

- Algorithm: HMAC-SHA256 with a random per-key salt
- Formula: `HMAC-SHA256(key: salt, message: rawApiKey)`
- API keys are high-entropy random strings — a fast hash is sufficient (no bcrypt/argon2 needed)

#### Validation Flow

1. Extract `X-Api-Key` from request header
2. Check in-memory cache (keyed by SHA-256 of the raw key)
3. On cache miss: load active keys from DB, compute HMAC-SHA256 with each key's salt, compare
4. On match: cache the result with a sliding expiration, authenticate the request
5. On no match: return `401 Unauthorized`

#### Caching

- Cache key: SHA-256 of the incoming raw key (raw key is never stored in cache)
- Sliding expiration: 30 minutes

#### Seeding

- On startup, if the `ApiKeys` table is empty, reads `API_KEY` from environment variables
- Generates a random salt, computes the HMAC-SHA256 hash, inserts a row with `ClientName="mcp-server"`
- The raw key passes through memory only once — the DB stores only hash + salt

### Endpoints

| Method | Route           | Description              |
|--------|-----------------|--------------------------|
| GET    | /api/tasks      | List tasks (with filters)|
| GET    | /api/tasks/{id} | Get a single task        |
| POST   | /api/tasks      | Create a task            |
| PUT    | /api/tasks/{id} | Update a task            |
| DELETE | /api/tasks/{id} | Delete a task            |

Query filter parameters for `GET /api/tasks`: `status`, `priority`, `dueDateFrom`, `dueDateTo` (all optional).

---

## MCP Server (TaskManager.Mcp)

Streamable HTTP transport, stateless mode. Bridges MCP clients to TaskManager.Api.

### Layer Flow

```
MCP Client → Streamable HTTP (POST / SSE)
    ↓
Tool / Resource / Prompt  — MCP surface: attribute binding, parameter parsing
    ↓
Service                   — orchestration; no MCP types, no HttpClient
    ↓
Collaborator              — typed HttpClient; X-Api-Key set once at DI registration
    ↓
HttpClient → TaskManager.Api (REST)
```

### Tools

| Tool            | Description                                          | API Call               |
|-----------------|------------------------------------------------------|------------------------|
| `get_task`      | Get a single task by ID                              | GET /api/tasks/{id}    |
| `get_all_tasks` | List tasks with optional filters                     | GET /api/tasks         |
| `add_task`      | Create a new task                                    | POST /api/tasks        |
| `update_task`   | Update an existing task                              | PUT /api/tasks/{id}    |
| `delete_task`   | Delete a task                                        | DELETE /api/tasks/{id} |

### Resources

| URI                   | API Call                                                        |
|-----------------------|-----------------------------------------------------------------|
| `tasks://all`         | GET /api/tasks                                                  |
| `tasks://completed`   | GET /api/tasks?status=Completed                                 |
| `tasks://in-progress` | GET /api/tasks?status=InProgress                                |
| `tasks://today`       | GET /api/tasks?dueDateFrom={today}&dueDateTo={today}            |

### Prompts

| Prompt             | Description                                               |
|--------------------|-----------------------------------------------------------|
| `daily-plan`       | Top 3 highest-priority tasks due today                    |
| `prioritize-tasks` | Reviews open tasks and suggests a prioritized order       |

### Configuration

| Variable         | Description                                        | Example               |
|------------------|----------------------------------------------------|-----------------------|
| `API_BASE_URL`   | Base URL of TaskManager.Api inside compose network | `http://api:8080`     |
| `API_KEY`        | Raw API key, forwarded as `X-Api-Key`              | *(from `.env`)*       |
| `ASPNETCORE_URLS`| MCP server bind URL inside the container           | `http://0.0.0.0:8080` |

### Client Registration

```jsonc
{
  "mcpServers": {
    "task-manager": {
      "url": "http://localhost:5050/mcp"
    }
  }
}
```

---

## Docker Setup

All services run inside Docker containers orchestrated by Docker Compose. No local .NET, Python, or PostgreSQL installation is required.

- Secrets (API key, DB credentials) are provided via a `.env` file — never committed to source control.
- EF Core migrations run automatically on API container start.
- The MCP server is registered in Claude Code's MCP config using `docker compose run --rm mcp`.
