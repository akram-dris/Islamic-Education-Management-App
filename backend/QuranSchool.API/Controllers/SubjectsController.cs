using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Subjects.Create;
using QuranSchool.Application.Features.Subjects.GetAll;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route("api/subjects")]
public class SubjectsController : ApiController
{
    private readonly ISender _sender;

    public SubjectsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSubjectsQuery(), cancellationToken);
        return HandleResult(result);
    }
}
