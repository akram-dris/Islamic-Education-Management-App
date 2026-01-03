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
using QuranSchool.Application.Features.Assignments.Create;
using QuranSchool.Application.Features.Assignments.GetMy;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Assignments;

public class AssignmentsControllerTests : FunctionalTest
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
    public async Task Create_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_cre", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Cre"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Cre"))).Content.ReadFromJsonAsync<Guid>();
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();

        SetAuthToken(await AuthenticateAsync("t_cre", "Pass123!"));
        var command = new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/assignments", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(Guid.NewGuid(), "T", "D", DateOnly.FromDateTime(DateTime.Now)));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/assignments/{Guid.NewGuid()}", new UpdateAssignmentRequest("T", "D", DateOnly.FromDateTime(DateTime.Now)));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.DeleteAsync($"api/assignments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        var request = new UpdateAssignmentRequest("Title", "Desc", DateOnly.FromDateTime(DateTime.Now));

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/assignments/{Guid.NewGuid()}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_asg", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Asg"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Asg"))).Content.ReadFromJsonAsync<Guid>();
        
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();

        var createResponse = await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))));
        var assignmentId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var request = new UpdateAssignmentRequest("New Title", "New Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(2)));

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/assignments/{assignmentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMy_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync("api/assignments/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMy_ShouldReturnAssignments_WhenAuthenticatedAsStudent()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_asg", "Pass123!", "S", UserRole.Student));

        var studentToken = await AuthenticateAsync("s_asg", "Pass123!");
        SetAuthToken(studentToken);

        // Act
        var response = await HttpClient.GetAsync("api/assignments/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenAssignmentExists()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_asg_del", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Asg_Del"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Asg_Del"))).Content.ReadFromJsonAsync<Guid>();
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}