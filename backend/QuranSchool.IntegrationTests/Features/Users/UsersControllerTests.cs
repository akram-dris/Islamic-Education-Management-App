using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Controllers;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Users;

public class UsersControllerTests : FunctionalTest
{
    private async Task AuthenticateAsAdminAsync()
    {
        var loginCommand = new LoginCommand("admin", "admin123");
        var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginCommand);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var registerResponse = await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("upd_user", "Pass123!", "Old Name", UserRole.Student));
        var userId = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        var request = new UpdateUserRequest("New Name", "NewPass123!");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LinkStudent_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var pId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("p_link", "Pass123!", "P", UserRole.Parent))).Content.ReadFromJsonAsync<Guid>();
        var sId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_link", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/users/{pId}/students", sId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var command = new RegisterUserCommand("newuser", "Password123!", "New User", UserRole.Teacher);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userId = await response.Content.ReadFromJsonAsync<Guid>();
        userId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnUsers_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync("api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedList<UserResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        
        // Create a user to delete
        var registerCommand = new RegisterUserCommand("deleteme", "Password123!", "Delete Me", UserRole.Student);
        var registerResponse = await HttpClient.PostAsJsonAsync("api/users/register", registerCommand);
        var userId = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var command = new RegisterUserCommand("unauthorized", "Password123!", "Unauthorized", UserRole.Teacher);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUsernameIsEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var command = new RegisterUserCommand("", "Password123!", "Full Name", UserRole.Student);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Validation Error");
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync("api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/users/{Guid.NewGuid()}", new UpdateUserRequest("N", "P"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.DeleteAsync($"api/users/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LinkStudent_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/users/{Guid.NewGuid()}/students", Guid.NewGuid());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LinkStudent_ShouldReturnNotFound_WhenParentNotFound()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/users/{Guid.NewGuid()}/students", Guid.NewGuid());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/users/{Guid.NewGuid()}", new UpdateUserRequest("N", "P"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
