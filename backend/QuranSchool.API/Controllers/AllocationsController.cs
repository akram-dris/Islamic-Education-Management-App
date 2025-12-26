using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Features.Allocations.Create;
using QuranSchool.Application.Features.Allocations.GetAll;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route("api/allocations")]
public class AllocationsController : ApiController
{
    private readonly ISender _sender;

    public AllocationsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAllocationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllAllocationsQuery(), cancellationToken);
        return HandleResult(result);
    }
}
