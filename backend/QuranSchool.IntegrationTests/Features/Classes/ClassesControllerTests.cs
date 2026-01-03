using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Controllers;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Application.Features.Classes.GetAll;
using QuranSchool.Application.Features.Classes.Update;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Classes;

public class ClassesControllerTests : FunctionalTest
{
    private async Task AuthenticateAsAdminAsync()
    {
        var loginCommand = new LoginCommand("admin", "admin123");
        var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginCommand);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var command = new CreateClassCommand("New Class");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/classes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var classId = await response.Content.ReadFromJsonAsync<Guid>();
        classId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var command = new CreateClassCommand("");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/classes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ShouldReturnClasses()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("Class 1"));

        // Act
        var response = await HttpClient.GetAsync("api/classes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ClassResponse>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("Old Name"));
        var classId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var request = new UpdateClassRequest("New Name");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/classes/{classId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenClassExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("To Delete"));
        var classId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/classes/{classId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var name = "Duplicate Class";
        await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand(name));

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand(name));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails!.Title.Should().Be("Conflict");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenClassDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdateClassRequest("New Name");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/classes/{Guid.NewGuid()}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ShouldReturnForbidden_WhenUserIsStudent()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        await HttpClient.PostAsJsonAsync("api/users/register", new QuranSchool.Application.Features.Users.Register.RegisterUserCommand("student_cls", "student123", "Student", QuranSchool.Domain.Enums.UserRole.Student));

        var studentToken = await AuthenticateAsync("student_cls", "student123");
        SetAuthToken(studentToken);
        var command = new CreateClassCommand("Student Class");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/classes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<string> AuthenticateAsync(string username, string password)
    {
        var loginCommand = new LoginCommand(username, password);
        var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginCommand);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!.Token;
    }

    private void SetAuthToken(string token)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
