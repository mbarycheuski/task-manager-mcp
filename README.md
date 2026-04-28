# Task Manager MCP

A single-user task management system exposing a REST API and MCP (Model Context Protocol) server, both built in C# .NET 10.

## Overview

Task Manager MCP consists of two integrated components:

1. **TaskManager.Api** — A REST API for task management with PostgreSQL persistence
2. **TaskManager.Mcp** — An MCP server that bridges Claude and other LLM clients to the API

Manage your tasks through REST endpoints, or let Claude handle your task management via the MCP interface.

## Tech Stack

| Component | Technology |
|-----------|-----------|
| API | C# / .NET 10 (ASP.NET Core) |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Validation | FluentValidation |
| MCP Server | ModelContextProtocol.AspNetCore |
| Transport | HTTP (Streamable, stateless) |
| Infrastructure | Docker / Docker Compose |

## Prerequisites

- **WSL2** (Windows Subsystem for Linux 2) with Docker installed
- **.NET 10 SDK** (for local development/testing, not required for Docker)
- **PowerShell** (on Windows; for `run.ps1` scripts)

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/mbarycheuski/task-manager-mcp
cd task-manager-mcp
```

### 2. Configure Environment

Copy the example environment file and configure your secrets:

```powershell
Copy-Item .env.example .env
```

Edit `.env` and set the required values:

```env
API_KEY=<your-secret-api-key>              # Required — random high-entropy string
POSTGRES_PASSWORD=<your-db-password>       # Required — secure database password
MCP_PROXY_AUTH_TOKEN=<your-mcp-token>      # Required — secures the MCP Inspector endpoint
```

**Important**: Never commit `.env` to source control.

### 3. Start Services

```powershell
.\run.ps1              # Start services
.\run.ps1 -Build       # Rebuild images before starting
.\run.ps1 -Down        # Stop and remove containers
.\run.ps1 -SyncCerts   # Sync Windows CA certificates to WSL (run once)
```

### 4. Verify Installation

Once containers are running:

- **API Swagger UI**: http://localhost:8080/docs
- **MCP Server**: http://localhost:5050
- **MCP Inspector**: See the URL displayed by `run.ps1` (includes auth token)
- **Database**: PostgreSQL at `localhost:5432` (credentials from `.env`)

## MCP Integration

### Claude Code

A `.mcp.json` file is included in the repository, so Claude Code connects to the MCP server automatically — no manual registration needed.

### Other MCP Clients

Register the server in your client configuration:

```json
{
  "mcpServers": {
    "task-manager": {
      "type": "http",
      "url": "http://localhost:5050"
    }
  }
}
```

### Available Tools

- `get_all_tasks` — List tasks with optional filters
- `get_task` — Get a single task by ID
- `add_task` — Create a new task
- `update_task` — Update an existing task
- `delete_task` — Delete a task

### Available Resources

- `tasks://all` — All tasks
- `tasks://open` — Tasks with status `None` or `InProgress`
- `tasks://in-progress` — Tasks with status `InProgress`
- `tasks://completed` — Tasks with status `Completed`
- `tasks://today` — Tasks due today

### Available Prompts

- `daily-plan` — Top 3 highest-priority tasks that are overdue or due today
- `prioritize-tasks` — Reviews open tasks and suggests a prioritized order

## Testing

Test scenarios for MCP tools are located in [tests/scenarios/tools/](tests/scenarios/tools/). Each tool has comprehensive test coverage including happy path and error cases.

**Test Coverage:**
- `add_task` — 14 scenarios (happy path + validation errors)
- `delete_task` — 3 scenarios
- `get_all_tasks` — 8 scenarios (filters, boundaries, empty results)
- `get_task` — 8 scenarios (success, not found, idempotency)
- `update_task` — 13 scenarios (field updates, status changes, validation)

**Running Tests:**
Ask Claude: `/run-scenarios` to execute all MCP tool test scenarios. The `run-scenarios` agent will execute all test cases and report pass/fail results.

## Example Usage

> "Create a task to review the Q2 budget, mark it as high priority, due next week."
>
> Claude uses the `add_task` tool to create the task with your specifications.

> "Show me all in-progress tasks."
>
> Claude uses the `get_all_tasks` tool filtered by `InProgress` status.

> "What should I work on today?"
>
> Claude uses the `daily-plan` prompt to suggest your top priority tasks for today.
