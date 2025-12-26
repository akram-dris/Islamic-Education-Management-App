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

    /// <summary>
    /// Marks attendance for a specific session.
    /// </summary>
    /// <param name="command">The attendance records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost("mark")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Mark([FromBody] MarkAttendanceCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves the attendance history for the authenticated student.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of attendance records.</returns>
    [HttpGet("my")]
    [ProducesResponseType(typeof(List<AttendanceResponse>), StatusCodes.Status200OK)]
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
