using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.API.Abstractions;

[ApiController]
[EnableRateLimiting("fixed")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        if (result is IValidationResult validationResult)
        {
            return BadRequest(CreateProblemDetails(
                "Validation Error", 
                result.Error, 
                validationResult.Errors));
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(CreateProblemDetails("Validation Error", result.Error)),
            ErrorType.NotFound => NotFound(CreateProblemDetails("Not Found", result.Error)),
            ErrorType.Conflict => Conflict(CreateProblemDetails("Conflict", result.Error)),
            ErrorType.Unauthorized => Unauthorized(CreateProblemDetails("Unauthorized", result.Error)),
            ErrorType.Forbidden => Forbid(),
            _ => BadRequest(CreateProblemDetails("Bad Request", result.Error))
        };
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result is IValidationResult validationResult)
        {
            return BadRequest(CreateProblemDetails(
                "Validation Error", 
                result.Error, 
                validationResult.Errors));
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(CreateProblemDetails("Validation Error", result.Error)),
            ErrorType.NotFound => NotFound(CreateProblemDetails("Not Found", result.Error)),
            ErrorType.Conflict => Conflict(CreateProblemDetails("Conflict", result.Error)),
            ErrorType.Unauthorized => Unauthorized(CreateProblemDetails("Unauthorized", result.Error)),
            ErrorType.Forbidden => Forbid(),
            _ => BadRequest(CreateProblemDetails("Bad Request", result.Error))
        };
    }

    private static ProblemDetails CreateProblemDetails(
        string title, 
        Error error,
        Error[]? errors = null) =>
        new()
        {
            Title = title,
            Type = error.Code,
            Detail = error.Description,
            Status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            },
            Extensions = { { nameof(errors), errors } }
        };
}
