using AcmeTaskApi.Application.Common;
using AcmeTaskApi.Application.DTOs.Tasks;
using AcmeTaskApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcmeTaskApi.API.Controllers;

[Authorize] // Protege todos los endpoints de tareas
[Route("api/tasks")]
public class TasksController : ApiControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] PaginationParams pagination, [FromQuery] TaskFilterDto filter, CancellationToken ct)
    {
        var result = await _taskService.GetTasksAsync(CurrentUserId, pagination, filter, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(long id, CancellationToken ct)
    {
        var task = await _taskService.GetTaskByIdAsync(CurrentUserId, id, ct);
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto, CancellationToken ct)
    {
        var created = await _taskService.CreateTaskAsync(CurrentUserId, dto, ct);
        return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(long id, [FromBody] TaskUpdateDto dto, CancellationToken ct)
    {
        var updated = await _taskService.UpdateTaskAsync(CurrentUserId, id, dto, ct);
        return Ok(updated);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> PatchStatus(long id, [FromBody] TaskPatchStatusDto dto, CancellationToken ct)
    {
        await _taskService.PatchStatusAsync(CurrentUserId, id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(long id, CancellationToken ct)
    {
        await _taskService.DeleteTaskAsync(CurrentUserId, id, ct);
        return NoContent();
    }
}