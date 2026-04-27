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

Copy the example environment file and configure your API key:

```powershell
Copy-Item .env.example .env
```

Edit `.env` and set the required secrets:

```env
API_KEY=<your-secret-api-key>              # Required — random high-entropy string
POSTGRES_PASSWORD=<your-db-password>       # Required — secure database password
MCP_PROXY_AUTH_TOKEN=<your-mcp-token>      # Required — secures the MCP Inspector endpoint
```

**Important**: Never commit `.env` to source control.

### 3. Start Services

**Windows (PowerShell)**:

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


### MCP Integration

A `.mcp.json` file is already included in the repository, so Claude Code connects to the MCP server automatically — no manual registration needed.

For other MCP clients, register the server in your client configuration:

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

#### Available MCP Tools

- `get_all_tasks` — List tasks with optional filters
- `get_task` — Get a single task by ID
- `add_task` — Create a new task
- `update_task` — Update an existing task
- `delete_task` — Delete a task

#### Available MCP Resources

- `tasks://all` — All tasks
- `tasks://open` — Tasks with status `None` or `InProgress`
- `tasks://in-progress` — Tasks with status `InProgress`
- `tasks://completed` — Tasks with status `Completed`
- `tasks://today` — Tasks due today

#### Available MCP Prompts

- `daily-plan` — Suggests top 3 highest-priority tasks due on a given date; accepts an optional `date` argument (`yyyy-MM-dd`), defaults to today
- `prioritize-tasks` — Analyzes open tasks and suggests a prioritized order

### Example MCP Usage

Ask Claude:

> Run `/mcp__task-manager__daily-plan 2026-04-28` to get a suggested plan for `2026-04-28` top priority tasks.

> "Create a task to review the Q2 budget, mark it as high priority, due next week."
>
> Claude uses the `add_task` tool to create the task with your specifications.

> "Show me all in-progress tasks."
>
> Claude uses the `get_all_tasks` tool filtered by `InProgress` status.
