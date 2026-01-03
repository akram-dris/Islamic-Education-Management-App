using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Errors;

public class ExceptionTests : FunctionalTest
{
    [Fact]
    public async Task GetForbidden_ShouldReturnForbidden()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/forbidden");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetForbiddenGeneric_ShouldReturnForbidden()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/forbidden-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUnauthorized_ShouldReturnUnauthorized()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/unauthorized");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUnauthorizedGeneric_ShouldReturnUnauthorized()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/unauthorized-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetNotFound_ShouldReturnNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/notfound");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNotFoundGeneric_ShouldReturnNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/notfound-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetConflict_ShouldReturnConflict()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/conflict");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetConflictGeneric_ShouldReturnConflict()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/conflict-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetUnknown_ShouldReturnBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/unknown");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUnknownGeneric_ShouldReturnBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/unknown-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBadRequest_ShouldReturnBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/badrequest");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBadRequestGeneric_ShouldReturnBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/badrequest-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetValidationGeneric_ShouldReturnBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/validation-generic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails!.Title.Should().Be("Validation Error");
    }

    [Fact]
    public async Task GetValidation_ShouldReturnBadRequest()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/validation");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails!.Title.Should().Be("Validation Error");
    }

    [Fact]
    public async Task GetSuccess_ShouldReturnOk()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/success");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Test_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Act
        var response = await HttpClient.GetAsync("api/errors/test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails!.Title.Should().Be("Server Error");
    }
}
