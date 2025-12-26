using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Auth;

public class LoginTests : FunctionalTest
{
    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginCommand("admin", "admin123");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var command = new LoginCommand("admin", "wrongpassword");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
