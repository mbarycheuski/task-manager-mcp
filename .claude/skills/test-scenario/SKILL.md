---
name: test-scenario
description: Create or update test scenario files for MCP tools.
user-invocable: true
---

# test-scenario

Generate or update scenario markdown files under `tests/scenarios/tools/` for a given MCP tool.

## Usage

```
/test-scenario tool <name>
```

- `<name>` — the tool name (e.g. `add_task`, `update_task`, `delete_task`)

**Examples:**
```
/test-scenario tool add_task
/test-scenario tool update_task
/test-scenario tool delete_task
```

## Workflow

1. **Read the spec** — `docs/architecture.md` for the MCP interface.
2. **Check for an existing file** at the target path (see below). If it exists, read it first and preserve existing slugs and style.
3. **Derive scenarios from the spec only** — do not invent rules. Cover:
   - **Happy path** — meaningful input combinations (all fields, required-only, boundary values, each enum value)
   - **Validation errors** — one per documented rule, with the exact boundary value
   - **State transitions** — only for tools that change state
4. **Assign a stable slug** per scenario (e.g. `all_fields`, `empty_title`, `due_today`). Slugs must be unique within the file and never change once written.
5. **Write the file** using the format for tools below.

## File paths

| Type | Path |
|------|------|
| tool | `tests/scenarios/tools/<name>.md` |

---

## Format: tool scenarios

### Happy-path / state-modifying

````markdown
## Scenario `<slug>`: <short description>

### Goal
One sentence: what this verifies.

### Pre-conditions
- MCP server is running and reachable
- <other pre-conditions, if any>

### Steps

#### 1. Call `<tool>`

**Tool:** `<tool>`

**Inputs:**
```json
{ "title": "<name>_<slug>_<descriptive_suffix>", "notes": null, "priority": null, "dueDate": null }
```

*(Replace the title with the actual static string following the naming convention — e.g. `"add_task_all_fields_release_notes"`. All values must be hardcoded literals.)*

#### 2. Verify response

| Field         | Expected value                |
|---------------|-------------------------------|
| `id`          | any non-empty UUID            |
| `title`       | `"add_task_all_fields_release_notes"` *(exact literal used in step 1)* |
| `status`      | `"None"`                      |
| `createdAt`   | any valid UTC datetime        |
| `updatedAt`   | any valid UTC datetime        |
| `completedAt` | absent                        |

#### 3. Confirm persistence — call `get_task` *(state-modifying scenarios only)*

**Inputs:**
```json
{ "id": "<id from step 1>" }
```

**Expected:** response matches the field values from step 2.

### Pass criteria
- Tool returns a structured `TaskItem` with no errors
- All field values match exactly (timestamps must be non-empty, valid UTC)
- `get_task` (if applicable) returns the same task

### Cleanup
Call `delete_task` with the `id` from step 1.
````

### Validation error

````markdown
## Scenario `<slug>`: <short description>

### Goal
One sentence: what error is verified.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `<tool>` with <invalid input>

**Inputs:**
```json
{ "title": "", "notes": null, "priority": null, "dueDate": null }
```

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error code is `InvalidParams`
- Error message contains `"<expected substring from the spec>"`

### Cleanup
None — no task is created.
````

---

## Data naming convention

Every input that creates a task must use a globally unique `title`:

```
<name>_<slug>_<descriptive_suffix>
```

- `<name>` — tool name (e.g. `add_task`, `update_task`)
- `<slug>` — the scenario's slug, exactly
- `<descriptive_suffix>` — a short, meaningful phrase

**Examples:**
- `add_task_all_fields_release_notes`
- `add_task_required_only_minimal_task`
- `update_task_status_change_mark_complete`

For validation-error scenarios where the input itself is invalid (empty string, oversized value, bad enum), use the literal invalid value — the naming convention does not apply.

## Rules

- **Use only what the spec defines.** Do not assert on fields, validations, or behaviors not in `docs/architecture.md` or the MCP source.
- **Use clear matchers.** `any non-empty UUID`, `any valid UTC datetime`, `absent`, exact strings, or enum literals. No vague language.
- **Do not assert on things the LLM cannot observe** through MCP (response time, query counts, audit logs, memory, internal side effects).
- **One scenario per behavior.** No duplicates within a file.
- **No comments inside JSON blocks.**
- **For error scenarios, always include the error code** (`InvalidParams`, `NotFound`).
- **Always clean up created tasks** via `delete_task`.
- **All input values must be static literals.** Every JSON block must contain fully resolved, hardcoded values — no placeholders, no expressions, no runtime-generated data. The test runner must be able to copy a JSON block and call the tool without substituting anything. The only exception is an `id` captured from a prior step's response, which must be referenced as a named placeholder (e.g. `"<id from step 1>"`) with a clear label identifying which step produced it.
