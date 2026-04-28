# Scenarios: update_task

## Scenario `update_task_all_fields`: Update all task fields

### Goal
Verify that `update_task` successfully updates all editable fields on a task.

### Pre-conditions
- MCP server is running and reachable
- Task exists with initial values

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_all_fields_original"`, status `"None"`, priority `"Low"`, notes `"Original notes"`, dueDate `"2026-05-01"`
- Store the returned `id`

#### 2. Update all fields on the task

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_all_fields_modified",
  "status": "InProgress",
  "priority": "Critical",
  "notes": "Updated notes with more detail",
  "dueDate": "2026-06-15"
}
```

#### 3. Verify response

**Expected fields:**

| Field         | Expected value                      |
|---------------|-------------------------------------|
| `id`          | Matches input `id`                 |
| `title`       | `"update_task_all_fields_modified"` |
| `status`      | `"InProgress"`                     |
| `priority`    | `"Critical"`                       |
| `notes`       | `"Updated notes with more detail"`  |
| `dueDate`     | `"2026-06-15"`                     |
| `updatedAt`   | Later than original `createdAt`   |

#### 4. Verify persistence — call `get_task`

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Expected:** response matches step 3 exactly

### Pass criteria
- ✓ Step 2 returns `TaskItem` with all updated fields
- ✓ All field values match the input from step 2
- ✓ `updatedAt` timestamp is newer than `createdAt`
- ✓ Step 4 confirms persistence (same values returned)

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_title_only`: Update title while preserving other fields

### Goal
Verify that `update_task` can update the title while leaving other fields unchanged.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_title_original"`, status `"None"`, priority `"Medium"`, dueDate `"2026-05-20"`
- Store the returned `id`

#### 2. Update only the title

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_title_modified",
  "status": "None",
  "priority": null,
  "notes": null,
  "dueDate": null
}
```

#### 3. Verify response

**Expected:**
- `title` is updated to `"update_task_title_modified"`
- `status` remains `"None"`
- `priority` reverts to absence (was `"Medium"`)
- `dueDate` reverts to absence (was `"2026-05-20"`)
- `notes` remains absent

### Pass criteria
- ✓ `title` field changed to new value
- ✓ `status` unchanged
- ✓ Other fields are now absent (nulled out by explicit null in input)
- ✓ `id` unchanged

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_status_to_in_progress`: Update status from None to InProgress

### Goal
Verify that `update_task` correctly transitions task status to `InProgress`.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_status_in_progress_test"`, status `"None"`
- Store the returned `id`

#### 2. Update status to InProgress

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_status_in_progress_test",
  "status": "InProgress"
}
```

#### 3. Verify response

**Expected:**
- `status` is `"InProgress"`
- `completedAt` field is absent (task is not completed)

### Pass criteria
- ✓ `status` changed to `"InProgress"`
- ✓ `completedAt` is not present in response
- ✓ Other fields unchanged

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_status_to_completed`: Update status to Completed via InProgress transition

### Goal
Verify that `update_task` correctly transitions task status through the required path (None → InProgress → Completed) and sets the `completedAt` timestamp.

### Pre-conditions
- MCP server is running and reachable
- Completion requires the state path: None → InProgress → Completed

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_status_completed_test"`, status `"None"`
- Store the returned `id`

#### 2. Update status to InProgress (required intermediate step)

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_status_completed_test",
  "status": "InProgress"
}
```

#### 3. Update status to Completed

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_status_completed_test",
  "status": "Completed"
}
```

#### 4. Verify response

**Expected:**
- `status` is `"Completed"`
- `completedAt` field is present and is a valid UTC datetime
- `completedAt` is at or after the task creation time

### Pass criteria
- ✓ Step 2: status successfully changes to `"InProgress"`
- ✓ Step 3: status successfully changes to `"Completed"`
- ✓ `completedAt` is a valid ISO 8601 datetime string
- ✓ `completedAt` >= `createdAt`
- ✓ `updatedAt` is updated to reflect the final change

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_priority_levels`: Update task through all priority levels

### Goal
Verify that `update_task` correctly handles all valid priority values: `Low`, `Medium`, `High`, `Critical`.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_priority_levels_test"`, priority `"Low"`
- Store the returned `id`

#### 2. Update priority to Medium

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_priority_levels_test",
  "status": "None",
  "priority": "Medium"
}
```

#### 3. Update priority to High

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_priority_levels_test",
  "status": "None",
  "priority": "High"
}
```

#### 4. Update priority to Critical

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_priority_levels_test",
  "status": "None",
  "priority": "Critical"
}
```

#### 5. Verify final state with `get_task`

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Expected:**
- `priority` is `"Critical"`

### Pass criteria
- ✓ Step 2 updates priority to `"Medium"`
- ✓ Step 3 updates priority to `"High"`
- ✓ Step 4 updates priority to `"Critical"`
- ✓ Step 5 confirms final value is `"Critical"`

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_due_date_change`: Update task due date

### Goal
Verify that `update_task` correctly updates the task due date.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_due_date_test"`, dueDate `"2026-05-10"`
- Store the returned `id`

#### 2. Update the due date

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_due_date_test",
  "status": "None",
  "dueDate": "2026-07-20"
}
```

#### 3. Verify response

**Expected:**
- `dueDate` is `"2026-07-20"`

### Pass criteria
- ✓ `dueDate` updated to new value
- ✓ Other fields unchanged

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_invalid_empty_title`: Update with empty title is rejected

### Goal
Verify that `update_task` rejects attempts to set an empty or whitespace-only title.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_empty_title_test"`, status `"None"`
- Store the returned `id`

#### 2. Attempt to update with empty title

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "",
  "status": "None"
}
```

#### 3. Verify error response

**Expected:**
- Error is raised indicating title is required and cannot be empty
- Task is not updated

#### 4. Verify task unchanged with `get_task`

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Expected:**
- `title` is still `"update_task_empty_title_test"`

### Pass criteria
- ✓ Step 2 raises `ValidationException`
- ✓ Error message indicates "Title is required and cannot be empty"
- ✓ Step 4 confirms task was not modified

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_invalid_empty_id`: Update with empty ID is rejected

### Goal
Verify that `update_task` rejects an empty/null GUID as the task ID.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Attempt to update with empty ID

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "title": "some_title",
  "status": "None"
}
```

#### 2. Verify error response

**Expected:**
- Error is raised indicating Task ID cannot be empty

### Pass criteria
- ✓ Tool raises `ValidationException`
- ✓ Error message contains "Task ID cannot be empty"

### Cleanup
No cleanup needed.

---

## Scenario `update_task_invalid_status`: Update with invalid status is rejected

### Goal
Verify that `update_task` rejects invalid status values.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_invalid_status_test"`, status `"None"`
- Store the returned `id`

#### 2. Attempt to update with invalid status

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_invalid_status_test",
  "status": "InvalidStatus"
}
```

#### 3. Verify error response

**Expected:**
- Error is raised indicating status is not a valid value
- Task is not updated

#### 4. Verify task unchanged with `get_task`

**Tool:** `get_task`

**Inputs:**
```json
{
  "id": "<id from step 1>"
}
```

**Expected:**
- `status` is still `"None"`

### Pass criteria
- ✓ Step 2 raises `ValidationException`
- ✓ Error message indicates "Status is not a valid value"
- ✓ Step 4 confirms task was not modified

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_idempotency`: Multiple identical updates return same result

### Goal
Verify that `update_task` is idempotent—calling it multiple times with the same input produces the same result.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_idempotency_test"`, status `"None"`
- Store the returned `id`

#### 2. Update the task first time

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_idempotency_test_modified",
  "status": "InProgress",
  "priority": "High"
}
```

**Store response as `response_1`**

#### 3. Update the task second time (identical input)

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_idempotency_test_modified",
  "status": "InProgress",
  "priority": "High"
}
```

**Store response as `response_2`**

#### 4. Compare responses

**Expected:**
- `response_1` and `response_2` have identical non-timestamp fields
- `updatedAt` timestamps may differ slightly but should be very close

### Pass criteria
- ✓ `response_1.id === response_2.id`
- ✓ `response_1.title === response_2.title`
- ✓ `response_1.status === response_2.status`
- ✓ `response_1.priority === response_2.priority`
- ✓ Both calls succeed without side-effects

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_notes_max_length`: Update with maximum allowed notes length

### Goal
Verify that `update_task` accepts notes at the maximum allowed length.

### Pre-conditions
- MCP server is running and reachable
- Maximum notes length is 500 characters

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_notes_max_test"`, notes `"Initial"`
- Store the returned `id`

#### 2. Update with maximum length notes

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "update_task_notes_max_test",
  "status": "None",
  "notes": "<500-character string of valid content>"
}
```

#### 3. Verify response

**Expected:**
- Update succeeds
- `notes` field contains the full 500-character string

### Pass criteria
- ✓ Tool accepts the update
- ✓ Response includes the full notes content
- ✓ `get_task` confirms notes are persisted

### Cleanup
Delete the fixture task using `delete_task`.

---

## Scenario `update_task_title_max_length`: Update with maximum allowed title length

### Goal
Verify that `update_task` accepts titles at the maximum allowed length.

### Pre-conditions
- MCP server is running and reachable
- Maximum title length is 256 characters

### Steps

#### 1. Create a test fixture
- Create task with title `"update_task_title_max_test"`
- Store the returned `id`

#### 2. Update with maximum length title

**Tool:** `update_task`

**Inputs:**
```json
{
  "id": "<id from step 1>",
  "title": "<256-character string of valid content>",
  "status": "None"
}
```

#### 3. Verify response

**Expected:**
- Update succeeds
- `title` field contains the full 256-character string

### Pass criteria
- ✓ Tool accepts the update
- ✓ Response includes the full title content
- ✓ `get_task` confirms title is persisted

### Cleanup
Delete the fixture task using `delete_task`.
