using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Auth.Login;

namespace QuranSchool.API.Controllers;

[Route("api/auth")]
public class AuthController : ApiController
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
