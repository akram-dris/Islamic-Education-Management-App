using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Assignments.Create;
using QuranSchool.Application.Features.Assignments.GetMy;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Assignments.BaseRoute)]
public class AssignmentsController : ApiController
{
    private readonly ISender _sender;

    public AssignmentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new assignment for a class and subject.
    /// </summary>
    /// <param name="command">The assignment details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created assignment ID.</returns>
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves all assignments for the authenticated student.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of assignments.</returns>
    [HttpGet(ApiRoutes.Assignments.My)]
    [ProducesResponseType(typeof(List<AssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAssignments(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetMyAssignmentsQuery(userId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment to update.</param>
    /// <param name="request">The update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the update was performed.</returns>
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpPut("{assignmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid assignmentId, [FromBody] UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Assignments.Update.UpdateAssignmentCommand(
            assignmentId,
            request.Title,
            request.Description,
            request.DueDate);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes an assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the deletion was performed.</returns>
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpDelete("{assignmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid assignmentId, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Assignments.Delete.DeleteAssignmentCommand(assignmentId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public record UpdateAssignmentRequest(string Title, string? Description, DateOnly DueDate);
