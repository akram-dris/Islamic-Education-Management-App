using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Files;

public class FilesControllerTests : FunctionalTest
{
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

    [Fact]
    public async Task Upload_ShouldReturnOk_WhenFileIsValid()
    {
        // Arrange
        var token = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(token);

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent("test file content"u8.ToArray());
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileContent, "file", "test.txt");

        // Act
        var response = await HttpClient.PostAsync("api/files/upload", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenNoFileProvided()
    {
        // Arrange
        var token = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(token);

        using var content = new MultipartFormDataContent();

        // Act
        var response = await HttpClient.PostAsync("api/files/upload", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var token = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(token);

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Array.Empty<byte>());
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileContent, "file", "empty.txt");

        // Act
        var response = await HttpClient.PostAsync("api/files/upload", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
