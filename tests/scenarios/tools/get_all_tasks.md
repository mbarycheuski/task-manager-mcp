# Scenarios: get_all_tasks

## Scenario `no_filters_returns_all`: No filters—returns all tasks

### Goal
Verify that `get_all_tasks` returns all tasks in the database when called with no filter arguments.

### Pre-conditions
- MCP server is running and reachable
- Database contains at least 2 tasks with different statuses

### Steps

#### 1. Create test fixtures
- Create task 1: title `"get_all_tasks_fixture_1"`, status `"None"`, priority `"Low"`, dueDate `"2026-05-10"`
- Create task 2: title `"get_all_tasks_fixture_2"`, status `"InProgress"`, priority `"High"`, dueDate `"2026-05-15"`

#### 2. Call `get_all_tasks` with no filters

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": null,
  "dueDateFrom": null,
  "dueDateTo": null
}
```

#### 3. Verify response structure

**Expected:**
- Response is an array of `TaskItem` objects
- Array length is ≥ 2 (at least our two fixtures)
- Each object has required fields: `id`, `title`, `status`, `createdAt`, `updatedAt`

### Pass criteria
- ✓ Response is a valid JSON array
- ✓ Array contains at least 2 items
- ✓ Both fixture tasks are present in the response (matched by title)
- ✓ All items have valid UUID `id` fields
- ✓ All items have valid UTC datetime `createdAt` and `updatedAt` fields
- ✓ No HTTP error returned

### Cleanup
Delete the two fixture tasks by ID to clean up test data.

---

## Scenario `filter_by_status_single`: Filter by single status

### Goal
Verify that `get_all_tasks` correctly filters tasks by a single status value.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create test fixtures
- Create task 1: title `"get_all_tasks_status_test_1"`, status `"None"`
- Create task 2: title `"get_all_tasks_status_test_2"`, status `"InProgress"`
- Create task 3: title `"get_all_tasks_status_test_3"`, status `"None"`

#### 2. Call `get_all_tasks` with statuses filter

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": ["None"],
  "dueDateFrom": null,
  "dueDateTo": null
}
```

#### 3. Verify response

**Expected:**
- Response array contains only tasks with status `"None"`
- Response includes tasks 1 and 3 (by title match)
- Response does not include task 2 (status is `"InProgress"`)

| Field      | Expected value                                  |
|------------|------------------------------------------------|
| length     | 2                                             |
| [0].status | `"None"`                                       |
| [1].status | `"None"`                                       |

### Pass criteria
- ✓ Response array length is exactly 2
- ✓ Both items have status `"None"`
- ✓ Task titles match fixtures 1 and 3
- ✓ Task 2 is not in response

### Cleanup
Delete all three fixture tasks.

---

## Scenario `filter_by_status_multiple`: Filter by multiple statuses

### Goal
Verify that `get_all_tasks` correctly filters tasks when multiple statuses are provided (OR logic).

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create test fixtures
- Create task 1: title `"get_all_tasks_multi_status_1"`, status `"None"`
- Create task 2: title `"get_all_tasks_multi_status_2"`, status `"InProgress"`
- Create task 3: title `"get_all_tasks_multi_status_3"`, status `"Completed"`

#### 2. Call `get_all_tasks` with multiple statuses

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": ["None", "Completed"],
  "dueDateFrom": null,
  "dueDateTo": null
}
```

#### 3. Verify response

**Expected:**
- Response array contains tasks with status `"None"` OR `"Completed"`
- Response includes tasks 1 and 3
- Response does not include task 2 (status is `"InProgress"`)

### Pass criteria
- ✓ Response array length is exactly 2
- ✓ One item has status `"None"`, one has status `"Completed"`
- ✓ Task 2 is not in response
- ✓ Titles match fixtures 1 and 3

### Cleanup
Delete all three fixture tasks.

---

## Scenario `filter_by_due_date_range`: Filter by due date range

### Goal
Verify that `get_all_tasks` correctly filters tasks by due date range (inclusive on both ends).

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create test fixtures
- Create task 1: title `"get_all_tasks_date_test_1"`, dueDate `"2026-05-01"`
- Create task 2: title `"get_all_tasks_date_test_2"`, dueDate `"2026-05-15"`
- Create task 3: title `"get_all_tasks_date_test_3"`, dueDate `"2026-06-01"`

#### 2. Call `get_all_tasks` with date range filter

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": null,
  "dueDateFrom": "2026-05-10",
  "dueDateTo": "2026-05-20"
}
```

#### 3. Verify response

**Expected:**
- Response array contains only tasks with `dueDate` between `"2026-05-10"` and `"2026-05-20"` (inclusive)
- Response includes task 2 (dueDate `"2026-05-15"`)
- Response does not include task 1 (dueDate `"2026-05-01"`, before range)
- Response does not include task 3 (dueDate `"2026-06-01"`, after range)

### Pass criteria
- ✓ Response array length is 1
- ✓ Single item has title `"get_all_tasks_date_test_2"`
- ✓ Item dueDate is `"2026-05-15"`

### Cleanup
Delete all three fixture tasks.

---

## Scenario `filter_combined_status_and_date`: Combined status and date filters

### Goal
Verify that `get_all_tasks` correctly applies both status AND date filters simultaneously (AND logic).

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create test fixtures
- Create task 1: title `"get_all_tasks_combo_1"`, status `"None"`, dueDate `"2026-05-05"`
- Create task 2: title `"get_all_tasks_combo_2"`, status `"InProgress"`, dueDate `"2026-05-15"`
- Create task 3: title `"get_all_tasks_combo_3"`, status `"None"`, dueDate `"2026-05-25"`

#### 2. Call `get_all_tasks` with both filters

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": ["None"],
  "dueDateFrom": "2026-05-10",
  "dueDateTo": "2026-05-30"
}
```

#### 3. Verify response

**Expected:**
- Response includes only tasks with status `"None"` AND dueDate within `["2026-05-10", "2026-05-30"]`
- Response includes task 3 only
- Response excludes task 1 (dueDate too early) and task 2 (wrong status)

### Pass criteria
- ✓ Response array length is 1
- ✓ Single item has title `"get_all_tasks_combo_3"`
- ✓ Item status is `"None"` and dueDate is `"2026-05-25"`

### Cleanup
Delete all three fixture tasks.

---

## Scenario `date_range_boundary_inclusive`: Due date range boundaries are inclusive

### Goal
Verify that `dueDateFrom` and `dueDateTo` are inclusive (match tasks on exact boundary dates).

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Create test fixtures
- Create task 1: title `"get_all_tasks_boundary_1"`, dueDate `"2026-05-10"`
- Create task 2: title `"get_all_tasks_boundary_2"`, dueDate `"2026-05-20"`

#### 2. Call `get_all_tasks` with boundary dates

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": null,
  "dueDateFrom": "2026-05-10",
  "dueDateTo": "2026-05-20"
}
```

#### 3. Verify response

**Expected:**
- Response includes both tasks (boundaries are included)

### Pass criteria
- ✓ Response array length is 2
- ✓ Items have dueDates `"2026-05-10"` and `"2026-05-20"`

### Cleanup
Delete both fixture tasks.

---

## Scenario `empty_result`: Filters return empty result

### Goal
Verify that `get_all_tasks` returns an empty array when filters match no tasks.

### Pre-conditions
- MCP server is running and reachable

### Steps

#### 1. Call `get_all_tasks` with filters that match no tasks

**Tool:** `get_all_tasks`

**Inputs:**
```json
{
  "statuses": ["Completed"],
  "dueDateFrom": "2099-01-01",
  "dueDateTo": "2099-12-31"
}
```

#### 2. Verify response

**Expected:**
- Response is an empty array

### Pass criteria
- ✓ Response is a valid JSON array
- ✓ Array length is 0
- ✓ No error returned

### Cleanup
No cleanup needed (no fixtures created).
