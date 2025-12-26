using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Parents.GetChildren;
using QuranSchool.Application.Features.Assignments.GetMy;
using QuranSchool.Application.Features.Attendance.GetMy;
using QuranSchool.Application.Features.Users.GetAll;

namespace QuranSchool.API.Controllers;

[Authorize(Roles = "Parent")]
[Route("api/parents")]
public class ParentsController : ApiController
{
    private readonly ISender _sender;

    public ParentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves all children linked to the authenticated parent.
    /// </summary>
    /// <returns>A list of students.</returns>
    [HttpGet("children")]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyChildren(CancellationToken cancellationToken)
    {
        var parentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (parentIdClaim == null) return Unauthorized();

        var query = new GetChildrenQuery(Guid.Parse(parentIdClaim.Value));
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves assignments for a specific child.
    /// </summary>
    /// <param name="studentId">The ID of the child.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of assignments.</returns>
    [HttpGet("children/{studentId}/assignments")]
    [ProducesResponseType(typeof(List<AssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildAssignments(Guid studentId, CancellationToken cancellationToken)
    {
        // For MVP, we assume the parent is authorized to view this student's data 
        // if they are linked. A more robust check could be added here.
        var query = new GetMyAssignmentsQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves attendance records for a specific child.
    /// </summary>
    /// <param name="studentId">The ID of the child.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of attendance records.</returns>
    [HttpGet("children/{studentId}/attendance")]
    [ProducesResponseType(typeof(List<AttendanceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildAttendance(Guid studentId, CancellationToken cancellationToken)
    {
        var query = new GetMyAttendanceQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
