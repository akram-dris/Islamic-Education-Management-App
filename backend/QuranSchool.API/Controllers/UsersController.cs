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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserRole role, CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(role);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{parentId}/students")]
    public async Task<IActionResult> LinkStudent(Guid parentId, [FromBody] Guid studentId, CancellationToken cancellationToken)
    {
        var command = new LinkParentCommand(parentId, studentId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
