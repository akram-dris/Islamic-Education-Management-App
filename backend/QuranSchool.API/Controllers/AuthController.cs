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

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="command">The login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT token and user details if successful.</returns>
    /// <response code="200">Returns the token and user details.</response>
    /// <response code="400">If the credentials are invalid.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
