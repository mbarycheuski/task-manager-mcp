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
- Status lifecycle: `None → InProgress → Completed` (no backward transitions)
- Completing a task records `CompletedAt`
- `CreatedAt` and `UpdatedAt` are set automatically
- Any task can be deleted regardless of status

## Security

- API key is required for all API requests
- API key is stored in .NET user secrets (development) or environment variables (production)
- API key is **never** committed to source control or placed in `appsettings.json`
- MCP server reads the API key from environment variables
