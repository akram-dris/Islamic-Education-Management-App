using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Enrollments.Create;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Enrollments.BaseRoute)]
public class EnrollmentsController : ApiController
{
    private readonly ISender _sender;

    public EnrollmentsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Unenrolls a student from a class.
    /// </summary>
    /// <param name="enrollmentId">The ID of the enrollment to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the deletion was performed.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{enrollmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Enrollments.Delete.DeleteEnrollmentCommand(enrollmentId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
