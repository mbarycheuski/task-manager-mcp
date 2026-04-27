# Scenarios: delete_task

## Scenario `happy_path`: Successfully delete an existing task

### Goal
Verify that `delete_task` successfully deletes a task and returns without error.

### Pre-conditions
- MCP server is running and reachable
- A task exists in the database

### Steps

#### 1. Create a task — call `add_task`

**Tool:** `add_task`

**Inputs:**
```json
{
  "title": "delete_task_happy_path_test_item",
  "notes": null,
  "priority": null,
  "dueDate": null
}
```

**Expected:** Returns a `TaskItem` with an `id` field. Record this `id`.

#### 2. Delete the task — call `delete_task`

**Tool:** `delete_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

#### 3. Verify deletion — call `get_task`

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

### Pass criteria
- Step 2 returns successfully with no error
- Step 3 returns an error (the task no longer exists)

### Cleanup
None — the task was already deleted in step 2.

---

## Scenario `empty_id`: Empty ID (Guid.Empty)

### Goal
Verify that `delete_task` rejects an empty/zero Guid and returns a validation error.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `delete_task` with an empty Guid

**Tool:** `delete_task`

**Inputs:**
```json
{
  "id": "00000000-0000-0000-0000-000000000000"
}
```

### Pass criteria
- Tool returns an error (no successful deletion)
- Error code is `InvalidParams`
- Error message contains `"Task ID"`

### Cleanup
None — no task is affected.

---

## Scenario `not_found`: Task does not exist

### Goal
Verify that `delete_task` returns a `NotFoundException` when attempting to delete a non-existent task.

### Pre-conditions
- MCP server is running and reachable
- A valid, non-existent task ID is available (a UUID that was never created)

### Steps

#### 1. Call `delete_task` with a non-existent task ID

**Tool:** `delete_task`

**Inputs:**
```json
{
  "id": "11111111-1111-1111-1111-111111111111"
}
```

### Pass criteria
- Tool returns an error
- Error message indicates the resource was not found

### Cleanup
None — no task is affected.
