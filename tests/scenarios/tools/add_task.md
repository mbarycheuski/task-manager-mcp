# Scenarios: add_task

## Scenario `all_fields`: All fields provided

### Goal
Verify that `add_task` creates a task with all fields and returns the correct output.

### Pre-conditions
- MCP server is running and reachable
- Database is clean or at least does not contain a task with the same title

### Steps

#### 1. Call `add_task`

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_all_fields_release_notes",
  "notes": "Cover all breaking changes from v2 to v3.",
  "priority": "High",
  "dueDate": "2026-05-01"
}
```

### 2. Verify response

**Expected fields:**

| Field         | Expected value                                        |
|---------------|-------------------------------------------------------|
| `id`          | any non-empty UUID                                    |
| `title`       | `"add_task_all_fields_release_notes"`                   |
| `status`      | `"None"`                                              |
| `notes`       | `"Cover all breaking changes from v2 to v3."`         |
| `priority`    | `"High"`                                              |
| `dueDate`     | `"2026-05-01"`                                        |
| `createdAt`   | any valid UTC datetime                                |
| `updatedAt`   | any valid UTC datetime                                |
| `completedAt` | absent (omitted from JSON)                            |

### 3. Confirm persistence — call `get_task`

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Expected:** response matches the same field values as step 2.

### Pass criteria
- Step 1 returns a structured `TaskItem` output with no errors
- All field values in step 2 match exactly (except timestamps, which must be non-empty)
- Step 3 returns the same task without error

### Cleanup
Call `delete_task` with the `id` from step 1 to remove the test record.

---

## Scenario `title_only`: Title only (optional fields omitted)

### Goal
Verify that `add_task` succeeds with only the required `title` field, and that optional fields are absent from the response.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task`

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_title_only_minimal_task",
  "notes": null,
  "priority": null,
  "dueDate": null
}
```

#### 2. Verify response

| Field         | Expected value         |
|---------------|------------------------|
| `id`          | any non-empty UUID     |
| `title`       | `"add_task_title_only_minimal_task"` |
| `status`      | `"None"`               |
| `notes`       | absent                 |
| `priority`    | absent                 |
| `dueDate`     | absent                 |
| `createdAt`   | any valid UTC datetime |
| `updatedAt`   | any valid UTC datetime |
| `completedAt` | absent                 |

### Pass criteria
- Tool returns a `TaskItem` with no errors
- Optional fields (`notes`, `priority`, `dueDate`, `completedAt`) are absent from the JSON response

### Cleanup
Call `delete_task` with the `id` from step 1.

---

## Scenario `due_date_today`: Due date is today

### Goal
Verify that a due date equal to today is accepted (boundary — must be today or later).

### Pre-conditions
- MCP server is running and reachable
- Know today's date in `yyyy-MM-dd` format

### Steps

#### 1. Call `add_task` with `dueDate` set to today

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_due_date_today_review_task",
  "notes": null,
  "priority": null,
  "dueDate": "<today's date, e.g. 2026-04-27>"
}
```

#### 2. Verify response

| Field     | Expected value              |
|-----------|-----------------------------|
| `status`  | `"None"`                    |
| `dueDate` | today's date in `yyyy-MM-dd` |

### Pass criteria
- Tool returns successfully with no errors
- `dueDate` in response matches the date supplied

### Cleanup
Call `delete_task` with the `id` from step 1.

---

## Scenario `empty_title`: Title is empty — validation error

### Goal
Verify that `add_task` rejects an empty title with an `InvalidParams` error.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with an empty title

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "",
  "notes": null,
  "priority": null,
  "dueDate": null
}
```

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error message contains `"Title is required and cannot be empty"`
- Error code is `InvalidParams`

### Cleanup
None — no task is created.

---

## Scenario `title_too_long`: Title exceeds 255 characters — validation error

### Goal
Verify that `add_task` rejects a title longer than 255 characters.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with a 256-character title

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
  "notes": null,
  "priority": null,
  "dueDate": null
}
```

> The title above is exactly 256 `A` characters.

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error message contains `"Title cannot exceed 255 characters"`
- Error code is `InvalidParams`

### Cleanup
None — no task is created.

---

## Scenario `notes_too_long`: Notes exceed 4000 characters — validation error

### Goal
Verify that `add_task` rejects notes longer than 4000 characters.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with notes of 4001 characters

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_notes_too_long_test_task",
  "notes": "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAA",
  "priority": null,
  "dueDate": null
}
```

> The notes above are exactly 4001 `A` characters.

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error message contains `"Notes cannot exceed 4000 characters"`
- Error code is `InvalidParams`

### Cleanup
None — no task is created.

---

## Scenario `invalid_priority`: Invalid priority value — validation error

### Goal
Verify that `add_task` rejects an unrecognised priority string.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with an invalid priority

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_invalid_priority_test_task",
  "notes": null,
  "priority": "Urgent",
  "dueDate": null
}
```

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error code is `InvalidParams`

### Cleanup
None — no task is created.

---

## Scenario `whitespace_title`: Title is whitespace only — validation error

### Goal
Verify that `add_task` rejects a title containing only whitespace characters.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with a whitespace-only title

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "   \t\n  ",
  "notes": null,
  "priority": null,
  "dueDate": null
}
```

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error message contains `"Title is required and cannot be empty"`
- Error code is `InvalidParams`

### Cleanup
None — no task is created.

---

## Scenario `title_max_length`: Title exactly 255 characters — boundary pass

### Goal
Verify that a title with exactly 255 characters (the maximum allowed) is accepted.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with a 255-character title

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
  "notes": null,
  "priority": null,
  "dueDate": null
}
```

> The title above is exactly 255 `A` characters.

### Pass criteria
- Tool returns successfully with a `TaskItem`
- `title` field in response matches the input

### Cleanup
Call `delete_task` with the `id` from step 1.

---

## Scenario `notes_max_length`: Notes exactly 4000 characters — boundary pass

### Goal
Verify that notes with exactly 4000 characters (the maximum allowed) are accepted.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `add_task` with notes of 4000 characters

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_notes_max_length_detailed_task",
  "notes": "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAA",
  "priority": null,
  "dueDate": null
}
```

> The notes above are exactly 4000 `A` characters.

### Pass criteria
- Tool returns successfully with a `TaskItem`
- `notes` field in response matches the input

### Cleanup
Call `delete_task` with the `id` from step 1.

---

## Scenario `all_valid_priorities`: All valid priority values

### Goal
Verify that `add_task` accepts all valid priority enum values: Low, Medium, High, Critical.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1a. Call `add_task` with priority "Low"

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_all_valid_priorities_low_minimal_task",
  "notes": null,
  "priority": "Low",
  "dueDate": null
}
```

#### 1b. Call `add_task` with priority "Medium"

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_all_valid_priorities_medium_minimal_task",
  "notes": null,
  "priority": "Medium",
  "dueDate": null
}
```

#### 1c. Call `add_task` with priority "High"

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_all_valid_priorities_high_minimal_task",
  "notes": null,
  "priority": "High",
  "dueDate": null
}
```

#### 1d. Call `add_task` with priority "Critical"

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_all_valid_priorities_critical_minimal_task",
  "notes": null,
  "priority": "Critical",
  "dueDate": null
}
```

#### 2. Verify each response

| Priority  | Expected value |
|-----------|----------------|
| Low       | `"Low"`        |
| Medium    | `"Medium"`     |
| High      | `"High"`       |
| Critical  | `"Critical"`   |

### Pass criteria
- All four calls return successfully with `TaskItem` responses
- Each response contains the correct `priority` field matching the input

### Cleanup
Call `delete_task` for each of the four task IDs returned in steps 1a–1d.

---

## Scenario `past_due_date`: Due date in the past — validation error

### Goal
Verify that `add_task` rejects a due date earlier than today.

### Pre-conditions
- MCP server is running and reachable
- Know today's date in `yyyy-MM-dd` format

### Steps

#### 1. Call `add_task` with a due date one day in the past

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "add_task_past_due_date_test_task",
  "notes": null,
  "priority": null,
  "dueDate": "<yesterday's date, e.g. 2026-04-26>"
}
```

### Pass criteria
- Tool returns an error (no `TaskItem` in response)
- Error message contains `"Due date cannot be in the past"`
- Error code is `InvalidParams`

### Cleanup
None — no task is created.
