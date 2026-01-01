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

    /// <summary>
    /// Updates a subject's details.
    /// </summary>
    /// <param name="subjectId">The ID of the subject to update.</param>
    /// <param name="request">The update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the update was performed.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{subjectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid subjectId, [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Subjects.Update.UpdateSubjectCommand(subjectId, request.Name);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a subject.
    /// </summary>
    /// <param name="subjectId">The ID of the subject to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the deletion was performed.</returns>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{subjectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid subjectId, CancellationToken cancellationToken)
    {
        var command = new Application.Features.Subjects.Delete.DeleteSubjectCommand(subjectId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public record UpdateSubjectRequest(string Name);
