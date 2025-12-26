using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Submissions.Create;
using QuranSchool.Application.Features.Submissions.Grade;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route("api/submissions")]
public class SubmissionsController : ApiController
{
    private readonly ISender _sender;

    public SubmissionsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var command = new CreateSubmissionCommand(
            request.AssignmentId,
            Guid.Parse(userIdClaim.Value),
            request.FileUrl);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost("{id}/grade")]
    public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionRequest request, CancellationToken cancellationToken)
    {
        var command = new GradeSubmissionCommand(id, request.Grade, request.Remarks);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public record CreateSubmissionRequest(Guid AssignmentId, string FileUrl);
public record GradeSubmissionRequest(decimal Grade, string? Remarks);
