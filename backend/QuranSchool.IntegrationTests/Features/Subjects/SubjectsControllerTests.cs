using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Controllers;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Subjects.Create;
using QuranSchool.Application.Features.Subjects.GetAll;
using QuranSchool.Application.Features.Subjects.Update;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Subjects;

public class SubjectsControllerTests : FunctionalTest
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
        var command = new CreateSubjectCommand("New Subject");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/subjects", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subjectId = await response.Content.ReadFromJsonAsync<Guid>();
        subjectId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnSubjects()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("Subject 1"));

        // Act
        var response = await HttpClient.GetAsync("api/subjects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<SubjectResponse>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("Old Name"));
        var subjectId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var request = new UpdateSubjectRequest("New Name");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/subjects/{subjectId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSubjectExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("To Delete"));
        var subjectId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/subjects/{subjectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var name = "Duplicate Subject";
        await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand(name));

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand(name));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
