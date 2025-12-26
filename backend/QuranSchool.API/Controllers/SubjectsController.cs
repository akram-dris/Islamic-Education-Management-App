using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Subjects.Create;
using QuranSchool.Application.Features.Subjects.GetAll;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Subjects.BaseRoute)]
public class SubjectsController : ApiController
{
    private readonly ISender _sender;

    public SubjectsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new academic subject.
    /// </summary>
    /// <param name="command">The subject details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created subject ID.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves all academic subjects.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of subjects.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<SubjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSubjectsQuery(), cancellationToken);
        return HandleResult(result);
    }
}
