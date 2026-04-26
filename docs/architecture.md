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
    /Collaborators           — typed HttpClient wrappers for TaskManager.Api
    /Collaborators/Dto       — DTOs mirroring API JSON response shapes
    /Common                  — shared services (ITimeService)
    /Inputs                  — input parameter types for tools/resources
    /Mappers                 — type mapping extensions (Dto → Output, etc.)
    /Outputs                 — output types returned to MCP clients
    /Prompts                 — [McpServerPromptType] / [McpServerPrompt]
    /Resources               — [McpServerResourceType] / [McpServerResource]
    /Services                — orchestration layer (no MCP or HTTP concerns)
    /Settings                — strongly-typed configuration (API base URL, API key)
    /Tools                   — [McpServerToolType] / [McpServerTool]
    /Utilities               — helper utilities (serializers, constants, validators)
    /Utilities/Serializers   — JSON and custom serialization logic
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

Query filter parameters for `GET /api/tasks`: `statuses` (multi-value list, optional), `priority`, `dueDateFrom`, `dueDateTo` (all optional).

---

## MCP Server (TaskManager.Mcp)

Streamable HTTP transport, stateless mode. Bridges MCP clients to TaskManager.Api.

### Layer Flow

```
MCP Client → Streamable HTTP (POST / SSE)
    ↓
Tool / Resource                    — MCP surface: attribute binding, input parameter parsing
    ↓
Service                            — orchestration: Dto ↔ Output mapping, business logic
    ↓
Collaborator                       — typed HttpClient; X-Api-Key set once at DI registration
    ↓
HttpClient → TaskManager.Api (REST) → returns Dto
    ↓
Mapper (Dto → Output)             — TaskItemMappingExtensions converts collaborator DTOs to output types
    ↓
Tool / Resource returns Output    — MCP client receives Output types
```

### Types and Flow

- **Inputs** (`/Inputs/CreateTaskInput`, etc.) — parameter types for tool methods; bound from MCP request attributes
- **Dto** (`/Collaborators/Dto/`) — JSON shapes matching API responses; mapped from HTTP responses
- **Outputs** (`/Outputs/TaskItem`, etc.) — types returned to MCP clients; structured content + schema
- **Mappers** (`/Mappers/TaskItemMappingExtensions`) — convert Dto → Output; called by Service layer before returning to tools/resources

### Tools

| Tool            | Description                                                                      | API Call               | Status       |
|-----------------|----------------------------------------------------------------------------------|------------------------|--------------|
| `get_all_tasks` | List tasks; optional filters: `statuses` (list), `dueDateFrom`, `dueDateTo` (inclusive) | GET /api/tasks         | ✅ Implemented |
| `get_task`      | Get a single task by ID              | GET /api/tasks/{id}    | ✅ Implemented |
| `add_task`      | Create a new task                    | POST /api/tasks        | ✅ Implemented |
| `update_task`   | Update an existing task              | PUT /api/tasks/{id}    | ✅ Implemented |
| `delete_task`   | Delete a task                        | DELETE /api/tasks/{id} | ✅ Implemented |

### Resources

| URI                   | API Call                                                        | Status         |
|-----------------------|-----------------------------------------------------------------|----------------|
| `tasks://all`         | GET /api/tasks                                                  | ✅ Implemented |
| `tasks://completed`   | GET /api/tasks?statuses=Completed                               | ✅ Implemented |
| `tasks://in-progress` | GET /api/tasks?statuses=InProgress                              | ✅ Implemented |
| `tasks://open`        | GET /api/tasks?statuses=None&statuses=InProgress                | ✅ Implemented |
| `tasks://today`       | GET /api/tasks?dueDateFrom={today}&dueDateTo={today}            | ✅ Implemented |

### Prompts

| Prompt             | Description                                               | Status             |
|--------------------|-----------------------------------------------------------|----------------|
| `daily-plan`       | Top 3 highest-priority tasks due today                    | ✅ Implemented |
| `prioritize-tasks` | Reviews open tasks and suggests a prioritized order       | ✅ Implemented |


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
      "type": "http",
      "url": "http://localhost:5050"
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
