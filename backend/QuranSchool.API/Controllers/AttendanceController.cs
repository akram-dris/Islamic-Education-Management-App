using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Application.Features.Attendance.GetMy;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Attendance.BaseRoute)]
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
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpPost(ApiRoutes.Attendance.Mark)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
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
    [HttpGet(ApiRoutes.Attendance.My)]
    [ProducesResponseType(typeof(List<AttendanceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAttendance(CancellationToken cancellationToken)
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

        var query = new GetMyAttendanceQuery(userId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an attendance session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to update.</param>
    /// <param name="request">The update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the update was performed.</returns>
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpPut("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid sessionId, [FromBody] UpdateAttendanceRequest request, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Attendance.Update.UpdateAttendanceSessionCommand(
            sessionId,
            request.Date,
            request.Records);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes an attendance session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the deletion was performed.</returns>
    [Authorize(Roles = RoleNames.TeacherOrAdmin)]
    [HttpDelete("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid sessionId, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Attendance.Delete.DeleteAttendanceSessionCommand(sessionId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public record UpdateAttendanceRequest(DateOnly Date, List<AttendanceRecordDto> Records);
