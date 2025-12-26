using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Application.Features.Classes.GetAll;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route("api/classes")]
public class ClassesController : ApiController
{
    private readonly ISender _sender;

    public ClassesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new academic class.
    /// </summary>
    /// <param name="command">The class details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created class ID.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClassCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves all academic classes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of classes.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClassResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllClassesQuery(), cancellationToken);
        return HandleResult(result);
    }
}
