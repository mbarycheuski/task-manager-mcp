# Task Manager MCP — Architecture

## Project Structure

```
/docs                        — project documentation
/src
  /api                       — C# .NET 10 CRUD API
    /Auth                    — API key authentication handler and options
    /Controllers             — API controllers (request/response handling)
    /Services                — business logic layer
    /Models                  — EF Core entity models
    /DTOs                    — request/response data transfer objects
    /Validators              — FluentValidation validators
    /Data                    — DbContext, migrations, and seeders
    Dockerfile               — API container image
  /mcp                       — Python FastMCP MCP server
    Dockerfile               — MCP server container image
docker-compose.yml           — orchestrates all services (api, db, mcp)
.env                         — secrets injected into containers (excluded from git)
```

---

## API (C# .NET 10)

### General

- .NET 10 controller-based API
- 3-layer architecture: **Controller (DTO) → Service (DTO) → DbContext**
- Single-user system — no user authentication/authorization
- Protected by API key (passed via `X-Api-Key` header)
- API keys are hashed (HMAC-SHA256 + per-key salt) and stored in the database — never stored as plaintext
- Validated keys are cached in-memory to avoid DB hits on every request
- Default API key is seeded on first startup from the `API_KEY` environment variable
- Swagger UI enabled, supporting API key input for testing
- Input validation via FluentValidation

### Architecture

```
X-Api-Key header    — validated by ApiKeyAuthenticationHandler (with in-memory cache)
    ↓
Controller          — receives HTTP requests, works with DTOs, delegates to Service
    ↓
Service             — business logic, works with DTOs, maps to/from entities
    ↓
DbContext (EF Core) — data access, works with entity models
```

- Controllers handle request/response mapping and validation
- Services contain business logic and DTO ↔ Entity mapping
- DbContext manages persistence — no repository layer needed for this scope

### API Key Authentication

All endpoints require a valid API key in the `X-Api-Key` header. Authentication uses ASP.NET Core's authentication pipeline via a custom `AuthenticationHandler`.

#### ApiKey Model

| Field     | Type     | Description                          |
|-----------|----------|--------------------------------------|
| Id        | Guid     | Unique identifier                    |
| Name      | string   | Label (e.g. "mcp-server")           |
| KeyHash   | string   | HMAC-SHA256 hash of the raw key      |
| Salt      | string   | Random per-key salt (base64)         |
| CreatedAt | DateTime | Auto-set on creation                 |
| IsActive  | bool     | Soft revocation flag (default: true) |

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

- Uses `IMemoryCache` to avoid DB queries on every request
- Cache key: SHA-256 of the incoming raw key (raw key is never stored in cache)
- Sliding expiration: 30 minutes
- Cache is invalidated naturally via TTL — acceptable for a single-user system

#### Seeding

- On startup, if the `ApiKeys` table is empty, the seeder reads `API_KEY` from environment variables
- Generates a random salt, computes the HMAC-SHA256 hash, and inserts a row with Name="mcp-server"
- The raw key passes through memory only once at startup — the DB stores only hash + salt

#### Components

| Component                     | Location    | Responsibility                                      |
|-------------------------------|-------------|-----------------------------------------------------|
| `ApiKey` entity               | Models      | EF Core entity                                      |
| `ApiKeyHasher`                | Services    | HMAC-SHA256 hashing and verification                 |
| `ApiKeyAuthenticationHandler` | Auth        | ASP.NET `AuthenticationHandler` for `X-Api-Key`     |
| `ApiKeyCacheService`          | Services    | In-memory cache wrapper for validated keys           |
| `ApiKeySeeder`                | Data        | Seeds default key on first startup from env var      |

### Data Storage

- PostgreSQL database
- Entity Framework Core as the ORM

### Task Model

| Field       | Type       | Description                                      |
|-------------|------------|--------------------------------------------------|
| Id          | Guid       | Unique identifier                                |
| Title       | string     | Task title (required, max 255 chars)             |
| Notes       | string?    | Optional notes (max 4000 chars)                  |
| Priority    | enum?      | Optional priority (Low, Medium, High, Critical)  |
| Status      | enum       | None, InProgress, Completed                      |
| DueDate     | DateOnly?  | Optional due date (date only, no time component) |
| CreatedAt   | DateTime   | Auto-set on creation                             |
| UpdatedAt   | DateTime   | Auto-set on update                               |
| CompletedAt | DateTime?  | Set when task is moved to Completed              |

### Endpoints

| Method | Route              | Description                |
|--------|--------------------|----------------------------|
| GET    | /api/tasks         | Get tasks (with filters)   |
| GET    | /api/tasks/{id}    | Get a single task          |
| POST   | /api/tasks         | Create a new task          |
| PUT    | /api/tasks/{id}    | Update an existing task    |
| DELETE | /api/tasks/{id}    | Delete a task              |

### Query Filters for GET /api/tasks

All filters are optional and can be combined:

| Parameter  | Type       | Description                                              |
|------------|------------|----------------------------------------------------------|
| status     | enum?      | Filter by status (None, InProgress, Completed)           |
| priority   | enum?      | Filter by priority (Low, Medium, High, Critical)         |
| dueDateFrom| DateOnly?  | Tasks with due date on or after this date (ISO 8601)     |
| dueDateTo  | DateOnly?  | Tasks with due date on or before this date (ISO 8601)    |

---

## MCP Server (Python FastMCP)

The MCP server acts as a bridge between Claude Code and the Task Manager API.

### Tools

Tools are functions that call the API:

| Tool             | Description              |
|------------------|--------------------------|
| `get_task`       | Get a single task by ID  |
| `get_all_tasks`  | Get all tasks            |
| `add_task`       | Create a new task        |
| `update_task`    | Update an existing task  |
| `delete_task`    | Delete a task            |

### Resources

Resources provide structured read access to data:

| URI                    | Description                 | API Call                                           |
|------------------------|-----------------------------|----------------------------------------------------|
| `tasks://all`          | All tasks                   | `GET /api/tasks`                                   |
| `tasks://completed`    | Completed tasks             | `GET /api/tasks?status=Completed`                  |
| `tasks://today`        | Tasks due today             | `GET /api/tasks?dueDateFrom={today}&dueDateTo={today}` (MCP server resolves `today` to ISO date, e.g. `2026-04-18`) |
| `tasks://in-progress`  | Tasks currently in progress | `GET /api/tasks?status=InProgress`                 |

### Prompts

Prompts are commands Claude can invoke:

| Prompt               | Description                                              |
|----------------------|----------------------------------------------------------|
| `/daily-plan`        | Gets the top 3 highest-priority tasks due today          |
| `/prioritize-tasks`  | Reviews open tasks and suggests a prioritized order      |

### Design Principles

- The MCP server must be easy for AI to use and understand
- Clear tool descriptions and parameter documentation
- Structured responses that Claude can reason about

---

## Docker Setup

All services run inside Docker containers orchestrated by Docker Compose. No local .NET, Python, or PostgreSQL installation is required.

- Secrets (API key, DB credentials) are provided via a `.env` file — never committed to source control.
- EF Core migrations run automatically on API container start.
- The MCP server is registered in Claude Code's MCP config using `docker compose run --rm mcp`.
