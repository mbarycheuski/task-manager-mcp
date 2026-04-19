---
name: Scaffold state — no upper layers yet
description: As of the entity-move refactor (April 2026), the API project only contains Data layer files. Controllers, Services, Repositories, Auth, DTOs, and Validators have not been implemented yet.
type: project
---

As of commit aa89786 and the subsequent entity-move refactor, the only `.cs` files in `src/api` are:

- `Program.cs`
- `Data/` — AppDbContext, entity models, EF configurations, migrations

**Why:** The project is scaffolded in stages; upper layers (Controllers → Services → Repositories → Auth) are pending.

**How to apply:** When reviewing namespace import completeness or checking for broken references in upper layers, note that those layers do not yet exist — the namespace change cannot have broken them. The relevant risk is only in `docs/architecture.md` (stale folder references) and in the entity files themselves (convention violations).
