using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Contracts;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    /// <summary>Returns all tasks, optionally filtered by status, priority, or due date range.</summary>
    /// <param name="taskQueryFilters">Optional filters by status, priority, and due date range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType<IEnumerable<TaskItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] TaskQueryFilters taskQueryFilters,
        CancellationToken cancellationToken
    )
    {
        var tasks = await taskService.GetAllAsync(taskQueryFilters, cancellationToken);

        return Ok(tasks);
    }

    /// <summary>Returns a single task by its id.</summary>
    /// <param name="id">The id of the task to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType<TaskItem>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(id, cancellationToken);

        return Ok(task);
    }

    /// <summary>Creates a new task.</summary>
    /// <param name="createTaskRequest">The properties for the new task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType<TaskItem>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest createTaskRequest,
        CancellationToken cancellationToken
    )
    {
        var task = await taskService.CreateAsync(createTaskRequest, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>Updates an existing task.</summary>
    /// <param name="id">The id of the task to update.</param>
    /// <param name="updateTaskRequest">The properties to apply to the task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<TaskItem>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest updateTaskRequest,
        CancellationToken cancellationToken
    )
    {
        var task = await taskService.UpdateAsync(id, updateTaskRequest, cancellationToken);

        return Ok(task);
    }

    /// <summary>Deletes a task by its id.</summary>
    /// <param name="id">The id of the task to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await taskService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
