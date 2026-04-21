# Task Manager MCP — Project Context

## Overview

A single-user task management system consisting of two components:

1. **C# .NET 10 CRUD API** — backend REST API with PostgreSQL storage
2. **Python FastMCP Server** — MCP (Model Context Protocol) server that connects LLM to the API

## Tech Stack Summary

| Component      | Technology                          |
|----------------|-------------------------------------|
| API            | C# / .NET 10                       |
| ORM            | Entity Framework Core               |
| Database       | PostgreSQL                          |
| Validation     | FluentValidation                    |
| API Docs       | Swagger / Swashbuckle               |
| MCP Server     | Python / FastMCP                    |
| API Client     | httpx or requests (Python)          |
| Infrastructure | Docker / Docker Compose             |

## Business Requirements

- Create a task with a title (required), optional notes, priority, and due date
- Status lifecycle: `None → InProgress → Completed`; `InProgress` can revert to `None`, but `Completed` is terminal (no transitions out)
- Completed tasks are immutable — they can only be deleted, not updated
- Completing a task records `CompletedAt`
- `CreatedAt` and `UpdatedAt` are set automatically
- Any task can be deleted, regardless of status

## Business Rules

### Create (`POST /api/tasks`)

- **Title** — required; trimmed; 1–255 chars; not whitespace-only
- **Notes** — optional; max 4000 chars
- **Priority** — optional; must be a defined enum value (`Low`, `Medium`, `High`, `Critical`)
- **Status** — always initialized to `None`; cannot be set by the caller
- **DueDate** — optional; if provided, must be **today or later** (past dates rejected)
- `CreatedAt` / `UpdatedAt` set by the server; `CompletedAt` remains null

### Update (`PUT /api/tasks/{id}`)

- **Completed tasks are immutable** — any update attempt returns `400 BusinessException` ("Cannot modify a completed task")
- **Status transitions** (enforced in the service layer):

  | From → To                    | Allowed |
  |------------------------------|---------|
  | `None` → `InProgress`        | ✅      |
  | `None` → `Completed`         | ❌ (must go through `InProgress`) |
  | `InProgress` → `None`        | ✅ (revert) |
  | `InProgress` → `Completed`   | ✅      |
  | `Completed` → *anything*     | ❌ (terminal) |

- On transition into `Completed` — set `CompletedAt = UtcNow`
- **DueDate on update** — if provided, must be today or later (same rule as create)
- Title / Notes / Priority validation same as on create
- `UpdatedAt` refreshed on every successful update

### Delete (`DELETE /api/tasks/{id}`)

- Any task can be deleted, including `Completed` ones
- Returns `404 NotFoundException` if the task does not exist

### Query filters (`GET /api/tasks`)

- `status`, `priority` — must be valid enum values if provided
- `dueDateFrom` ≤ `dueDateTo` when both are supplied
- Invalid filter values return `400 BusinessException`

### Error mapping

| Condition                                                             | Exception           | HTTP |
|-----------------------------------------------------------------------|---------------------|------|
| Task not found                                                        | `NotFoundException` | 404  |
| Rule violation (bad status transition, modify completed, past due date on create, `dueDateFrom > dueDateTo`) | `BusinessException` | 400  |
| Schema / field validation (length, required, enum, format)            | FluentValidation    | 400  |

## Security

- All API endpoints require a valid API key in the `X-Api-Key` header
- API keys are hashed (HMAC-SHA256 + random per-key salt) and stored in the database — never stored as plaintext
- On first startup, if the `ApiKeys` table is empty, the seeder reads the raw key from the `API_KEY` environment variable, hashes it, and inserts a record; the raw key is never persisted
- Validated keys are cached in-memory (sliding 30-minute TTL) to avoid DB hits on every request
- API keys are **never** committed to source control or placed in `appsettings.json`
- MCP server reads the raw API key from environment variables
