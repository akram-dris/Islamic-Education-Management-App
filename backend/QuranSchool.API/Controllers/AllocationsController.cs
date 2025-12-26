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

    /// <summary>
    /// Creates a new teacher allocation (links Teacher to Class and Subject).
    /// </summary>
    /// <param name="command">The allocation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created allocation ID.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAllocationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves all teacher allocations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of allocations.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<AllocationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllAllocationsQuery(), cancellationToken);
        return HandleResult(result);
    }
}
