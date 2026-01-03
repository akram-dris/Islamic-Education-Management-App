using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.API.Controllers;

[ApiController]
[Route("api/errors")]
public class ErrorsController : ApiController
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        throw new Exception("Test exception");
    }

    [HttpGet("forbidden")]
    public IActionResult GetForbidden() => HandleResult(Result.Failure(Error.Forbidden("Test.Forbidden", "Forbidden")));

    [HttpGet("forbidden-generic")]
    public IActionResult GetForbiddenGeneric() => HandleResult(Result<string>.Failure(Error.Forbidden("Test.Forbidden", "Forbidden")));

    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized() => HandleResult(Result.Failure(Error.Unauthorized("Test.Unauthorized", "Unauthorized")));

    [HttpGet("unauthorized-generic")]
    public IActionResult GetUnauthorizedGeneric() => HandleResult(Result<string>.Failure(Error.Unauthorized("Test.Unauthorized", "Unauthorized")));

    [HttpGet("notfound")]
    public IActionResult GetNotFound() => HandleResult(Result.Failure(Error.NotFound("Test.NotFound", "Not found")));

    [HttpGet("notfound-generic")]
    public IActionResult GetNotFoundGeneric() => HandleResult(Result<string>.Failure(Error.NotFound("Test.NotFound", "Not found")));

    [HttpGet("conflict")]
    public IActionResult GetConflict() => HandleResult(Result.Failure(Error.Conflict("Test.Conflict", "Conflict")));

    [HttpGet("conflict-generic")]
    public IActionResult GetConflictGeneric() => HandleResult(Result<string>.Failure(Error.Conflict("Test.Conflict", "Conflict")));

    [HttpGet("validation")]
    public IActionResult GetValidation()
    {
        var errors = new[] { Error.Validation("Test.Validation", "Validation failed") };
        return HandleResult(ValidationResult.WithErrors(errors));
    }

    [HttpGet("validation-generic")]
    public IActionResult GetValidationGeneric()
    {
        var errors = new[] { Error.Validation("Test.Validation", "Validation failed") };
        return HandleResult(ValidationResult<string>.WithErrors(errors));
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest() => HandleResult(Result.Failure(Error.Validation("Test.BadRequest", "Bad request")));

    [HttpGet("badrequest-generic")]
    public IActionResult GetBadRequestGeneric() => HandleResult(Result<string>.Failure(Error.Validation("Test.BadRequest", "Bad request")));

    [HttpGet("unknown")]
    public IActionResult GetUnknown() => HandleResult(Result.Failure(new Error("Test.Unknown", "Unknown", (ErrorType)999)));

    [HttpGet("unknown-generic")]
    public IActionResult GetUnknownGeneric() => HandleResult(Result<string>.Failure(new Error("Test.Unknown", "Unknown", (ErrorType)999)));

    [HttpGet("success")]
    public IActionResult GetSuccess() => HandleResult(Result.Success());
}
