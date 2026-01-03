using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Submissions.Create;
using QuranSchool.Application.Features.Submissions.Grade;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Submissions.BaseRoute)]
public class SubmissionsController : ApiController
{
    private readonly ISender _sender;

    public SubmissionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Submits an assignment for the authenticated student.
    /// </summary>
    /// <param name="request">The submission details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created submission ID.</returns>
    [Authorize(Roles = RoleNames.Student)]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Submit([FromBody] CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var command = new CreateSubmissionCommand(
            request.AssignmentId,
            userId,
            request.FileUrl);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Grades a student's submission.
    /// </summary>
    /// <param name="id">The ID of the submission.</param>
    /// <param name="request">The grading details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpPost(ApiRoutes.Submissions.Grade)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionRequest request, CancellationToken cancellationToken)
    {
        var command = new GradeSubmissionCommand(id, request.Grade, request.Remarks);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a submission.
    /// </summary>
    /// <param name="submissionId">The ID of the submission to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the deletion was performed.</returns>
    [Authorize] // Student (owner), Teacher, Admin
    [HttpDelete("{submissionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid submissionId, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Submissions.Delete.DeleteSubmissionCommand(submissionId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public record CreateSubmissionRequest(Guid AssignmentId, string FileUrl);
public record GradeSubmissionRequest(decimal Grade, string? Remarks);
