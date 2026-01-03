using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using QuranSchool.API.Controllers;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Application.Features.Subjects.Create;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Application.Features.Allocations.Create;
using QuranSchool.Application.Features.Allocations.GetAll;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Allocations;

public class AllocationsControllerTests : FunctionalTest
{
    private async Task AuthenticateAsAdminAsync()
    {
        var loginCommand = new LoginCommand("admin", "admin123");
        var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginCommand);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenAllocationAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        
        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_con", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Con"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Con"))).Content.ReadFromJsonAsync<Guid>();
        
        var command = new CreateAllocationCommand(teacherId, classId, subjectId);
        await HttpClient.PostAsJsonAsync("api/allocations", command);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/allocations", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        
        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_all", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var teacherId2 = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_all2", "Pass123!", "T2", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_All"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_All"))).Content.ReadFromJsonAsync<Guid>();
        
        var createResponse = await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId));
        var allocationId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var request = new UpdateAllocationRequest(teacherId2, classId, subjectId);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/allocations/{allocationId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync("api/allocations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllocations()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync("api/allocations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AllocationResponse>>();
        result.Should().NotBeNull();
    }
}