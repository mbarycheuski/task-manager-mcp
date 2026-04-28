# Scenarios: get_task

## Scenario `get_task_by_id_success`: Get task by valid ID

### Goal
Verify that `get_task` returns a task when given a valid task ID.

### Pre-conditions
- MCP server is running and reachable
- Database is accessible

### Steps

#### 1. Create a test fixture
- Create task with title `"get_task_success_fixture"`, priority `"High"`, dueDate `"2026-05-15"`
- Store the returned `id`

#### 2. Call `get_task` with the fixture task ID

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

#### 3. Verify response

**Expected fields:**

| Field         | Expected value                       |
|---------------|--------------------------------------|
| `id`          | Matches input `id` from step 2      |
| `title`       | `"get_task_success_fixture"`         |
| `status`      | `"None"`                            |
| `priority`    | `"High"`                            |
| `dueDate`     | `"2026-05-15"`                      |
| `createdAt`   | Valid UTC datetime (non-null)       |
| `updatedAt`   | Valid UTC datetime (non-null)       |

### Pass criteria
- ✓ HTTP status is 200 (success)
- ✓ Response is a valid `TaskItem` object
- ✓ `id` field matches the requested ID
- ✓ `title`, `priority`, `dueDate` match fixture values
- ✓ `createdAt` and `updatedAt` are valid ISO 8601 datetime strings
- ✓ `status` is `"None"` (initial state after creation)

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `get_task_nonexistent_id`: Get task with non-existent ID

### Goal
Verify that `get_task` returns an appropriate error when given a non-existent task ID.

### Pre-conditions
- MCP server is running and reachable
- ID `00000000-0000-0000-0000-000000000001` does not exist in the database

### Steps

#### 1. Call `get_task` with a non-existent ID

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "00000000-0000-0000-0000-000000000001"
}
```

#### 2. Verify error response

**Expected:**
- Error code or exception is raised (e.g., `NotFoundException` or similar)
- Error message indicates the task was not found

### Pass criteria
- ✓ MCP tool raises a `NotFoundException` or equivalent
- ✓ Error message contains "not found" or similar indicator
- ✓ No task data is returned

### Cleanup
No cleanup needed (no fixtures created).

---

## Scenario `get_task_empty_guid`: Get task with empty/null GUID

### Goal
Verify that `get_task` rejects an empty or null GUID.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `get_task` with empty GUID

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "00000000-0000-0000-0000-000000000000"
}
```

#### 2. Verify error response

**Expected:**
- Error is raised indicating invalid input
- Error message indicates GUID cannot be empty

### Pass criteria
- ✓ Tool raises a validation error (e.g., `ValidationException`)
- ✓ Error message indicates "Task ID cannot be empty" or similar
- ✓ No database query is attempted

### Cleanup
No cleanup needed.

---

## Scenario `get_task_after_update`: Get task reflects recent updates

### Goal
Verify that `get_task` returns the updated values after a task has been modified.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"get_task_update_test"`, status `"None"`, priority `"Low"`, dueDate `"2026-05-10"`
- Store the returned `id`

#### 2. Update the task using `update_task`

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "get_task_update_test_modified",
  "status": "InProgress",
  "priority": "Critical",
  "dueDate": "2026-06-01"
}
```

#### 3. Call `get_task` to verify updates

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

#### 4. Verify response reflects updates

**Expected fields:**

| Field      | Expected value                    |
|-----------|-----------------------------------|
| `id`       | Same as input                    |
| `title`    | `"get_task_update_test_modified"` |
| `status`   | `"InProgress"`                    |
| `priority` | `"Critical"`                      |
| `dueDate`  | `"2026-06-01"`                    |
| `updatedAt`| Later timestamp than `createdAt` |

### Pass criteria
- ✓ Step 3 response matches all updated values from step 2
- ✓ `updatedAt` timestamp is later than `createdAt`
- ✓ `id` remains unchanged

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `get_task_completed_status`: Get task with Completed status

### Goal
Verify that `get_task` correctly returns and displays the `Completed` status along with `completedAt` timestamp.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"get_task_completed_test"`
- Store the returned `id`

#### 2. Update task to Completed status

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "get_task_completed_test",
  "status": "Completed"
}
```

#### 3. Call `get_task` to retrieve the completed task

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

#### 4. Verify response shows completed status

**Expected:**
- `status` field is `"Completed"`
- `completedAt` field is present and contains a valid UTC datetime
- `completedAt` is later than or equal to `createdAt`

### Pass criteria
- ✓ `status` is `"Completed"`
- ✓ `completedAt` is a valid ISO 8601 datetime
- ✓ `completedAt` >= `createdAt`
- ✓ All other fields unchanged from creation

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `get_task_idempotency`: Multiple calls to get_task return identical results

### Goal
Verify that repeated calls to `get_task` with the same ID return identical results (idempotent operation).

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"get_task_idempotency_test"`
- Store the returned `id`

#### 2. Call `get_task` first time

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Store response as `response_1`**

#### 3. Call `get_task` second time (no changes to task)

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Store response as `response_2`**

#### 4. Compare responses

**Expected:**
- `response_1` and `response_2` are byte-for-byte identical (excluding timestamps that may have minor precision differences)
- All non-timestamp fields are exactly equal

### Pass criteria
- ✓ `response_1.id === response_2.id`
- ✓ `response_1.title === response_2.title`
- ✓ `response_1.status === response_2.status`
- ✓ `response_1.priority === response_2.priority`
- ✓ `response_1.dueDate === response_2.dueDate`
- ✓ `response_1.createdAt === response_2.createdAt`
- ✓ `response_1.updatedAt === response_2.updatedAt`

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `get_task_with_notes`: Get task with long notes field

### Goal
Verify that `get_task` correctly returns tasks with the `notes` field populated and preserves formatting.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"get_task_notes_test"`, notes `"Line 1\nLine 2\nLine 3"`
- Store the returned `id`

#### 2. Call `get_task` to retrieve the task

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

#### 3. Verify response includes notes

**Expected:**
- `notes` field is present in response
- `notes` value is `"Line 1\nLine 2\nLine 3"` (exact match, preserving whitespace)

### Pass criteria
- ✓ `notes` field is present
- ✓ `notes` value matches the fixture value exactly
- ✓ Newlines and formatting are preserved

### Cleanup
Delete the fixture task using `delete_task`.
