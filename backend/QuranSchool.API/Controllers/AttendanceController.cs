using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Application.Features.Attendance.GetMy;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route("api/attendance")]
public class AttendanceController : ApiController
{
    private readonly ISender _sender;

    public AttendanceController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost("mark")]
    public async Task<IActionResult> Mark([FromBody] MarkAttendanceCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAttendance(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var query = new GetMyAttendanceQuery(Guid.Parse(userIdClaim.Value));
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
