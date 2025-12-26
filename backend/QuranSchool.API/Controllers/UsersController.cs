using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Users.LinkParent;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Enums;

namespace QuranSchool.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/users")]
public class UsersController : ApiController
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="command">The user registration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created user.</returns>
    /// <response code="200">Returns the unique identifier of the user.</response>
    /// <response code="400">If validation fails or username is taken.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a list of users filtered by role.
    /// </summary>
    /// <param name="role">The role to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of users matching the role.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] UserRole role, CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(role);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Links a student to a parent.
    /// </summary>
    /// <param name="parentId">The ID of the parent.</param>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPost("{parentId}/students")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LinkStudent(Guid parentId, [FromBody] Guid studentId, CancellationToken cancellationToken)
    {
        var command = new LinkParentCommand(parentId, studentId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
