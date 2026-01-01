using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Allocations.Create;
using QuranSchool.Application.Features.Allocations.GetAll;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Allocations.BaseRoute)]
public class AllocationsController : ApiController
{
    private readonly ISender _sender;

    public AllocationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new teacher allocation (links Teacher to Class and Subject).
    /// </summary>
    /// <param name="command">The allocation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created allocation ID.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAllocationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves all teacher allocations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of allocations.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<AllocationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllAllocationsQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an allocation.
    /// </summary>
    /// <param name="allocationId">The ID of the allocation to update.</param>
    /// <param name="request">The update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the update was performed.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{allocationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid allocationId, [FromBody] UpdateAllocationRequest request, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Allocations.Update.UpdateAllocationCommand(
            allocationId, 
            request.TeacherId, 
            request.ClassId, 
            request.SubjectId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes an allocation.
    /// </summary>
    /// <param name="allocationId">The ID of the allocation to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the deletion was performed.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{allocationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid allocationId, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Allocations.Delete.DeleteAllocationCommand(allocationId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public record UpdateAllocationRequest(Guid TeacherId, Guid ClassId, Guid SubjectId);
