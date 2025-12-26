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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllClassesQuery(), cancellationToken);
        return HandleResult(result);
    }
}
