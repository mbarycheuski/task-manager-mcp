---
name: Project scaffolding state
description: As of 2026-04-19, Models/Data layers exist with entity configs extracted; application layers (Controllers/Services/Repositories/DTOs/Validators/Auth) are still empty
type: project
---

The src/ directory was introduced in the first development commit (2026-04-19). As of that same date, a refactoring commit extracted entity configurations and renamed ApiKey.Name → ApiKey.ClientName.

Current state:
- Models: TaskItem, ApiKey, and Enums are implemented; ApiKey uses `ClientName` (not `Name`)
- Data: AppDbContext delegates to ApplyConfigurationsFromAssembly; separate IEntityTypeConfiguration classes in Data/Configurations/ (TaskItemConfiguration, ApiKeyConfiguration)
- Migrations: InitialCreate (single migration, directly edited to use ClientName column name — acceptable for a fresh project)
- Infra: Dockerfile, docker-compose.yml, Program.cs, TaskManager.slnx, .env.example, run.ps1 are present
- Empty directories: Controllers/, Services/, Repositories/, DTOs/, Validators/, Auth/ — scaffolded but no implementation files yet

Known doc drift: docs/architecture.md ApiKey field table still shows `Name` (line 67) instead of `ClientName` — not yet updated after the rename.

**Why:** This is the scaffolding phase; application-layer implementation is the next milestone.

**How to apply:** When reviewing follow-up commits, verify that application layers get filled in consistently with established project conventions. Flag the architecture.md Name→ClientName discrepancy if not yet fixed.
