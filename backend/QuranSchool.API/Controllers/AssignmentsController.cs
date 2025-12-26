using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Assignments.Create;
using QuranSchool.Application.Features.Assignments.GetMy;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route("api/assignments")]
public class AssignmentsController : ApiController
{
    private readonly ISender _sender;

    public AssignmentsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAssignments(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var query = new GetMyAssignmentsQuery(Guid.Parse(userIdClaim.Value));
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
