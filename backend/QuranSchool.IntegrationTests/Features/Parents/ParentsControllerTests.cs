using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using QuranSchool.Application.Features.Assignments.GetMy;
using QuranSchool.Application.Features.Attendance.GetMy;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Parents.GetChildren;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Application.Features.Users.LinkParent;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Parents;

public class ParentsControllerTests : FunctionalTest
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
    public async Task GetChildren_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync("api/parents/children");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetChildren_ShouldReturnForbidden_WhenUserIsStudent()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_p_fob", "Pass123!", "S", UserRole.Student));

        SetAuthToken(await AuthenticateAsync("s_p_fob", "Pass123!"));

        // Act
        var response = await HttpClient.GetAsync("api/parents/children");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetChildren_ShouldReturnChildren_WhenLinked()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var parentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("p_test", "Pass123!", "P", UserRole.Parent))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_test", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        
        await HttpClient.PostAsJsonAsync($"api/users/{parentId}/students", studentId);

        var parentToken = await AuthenticateAsync("p_test", "Pass123!");
        SetAuthToken(parentToken);

        // Act
        var response = await HttpClient.GetAsync("api/parents/children");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().Contain(c => c.Id == studentId);
    }

    [Fact]
    public async Task GetChildAssignments_ShouldReturnEmptyList_WhenChildDoesNotExist()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("p_notfound", "Pass123!", "P", UserRole.Parent));
        SetAuthToken(await AuthenticateAsync("p_notfound", "Pass123!"));

        // Act: Use a random GUID for studentId
        var response = await HttpClient.GetAsync($"api/parents/children/{Guid.NewGuid()}/assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChildAssignments_ShouldReturnOk()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        var parentToken = await AuthenticateAsync("p_test", "Pass123!"); // Reuse from previous if possible or re-create
        
        // Ensure student exists
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_asg_p", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var parentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("p_asg_p", "Pass123!", "P", UserRole.Parent))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync($"api/users/{parentId}/students", studentId);

        SetAuthToken(await AuthenticateAsync("p_asg_p", "Pass123!"));

        // Act
        var response = await HttpClient.GetAsync($"api/parents/children/{studentId}/assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetChildAssignments_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync($"api/parents/children/{Guid.NewGuid()}/assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetChildAttendance_ShouldReturnEmptyList_WhenChildDoesNotExist()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("p_att_nf", "Pass123!", "P", UserRole.Parent));
        SetAuthToken(await AuthenticateAsync("p_att_nf", "Pass123!"));

        // Act
        var response = await HttpClient.GetAsync($"api/parents/children/{Guid.NewGuid()}/attendance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AttendanceResponse>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChildAttendance_ShouldReturnOk()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_att_p2", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var parentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("p_att_p2", "Pass123!", "P", UserRole.Parent))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync($"api/users/{parentId}/students", studentId);

        SetAuthToken(await AuthenticateAsync("p_att_p2", "Pass123!"));

        // Act
        var response = await HttpClient.GetAsync($"api/parents/children/{studentId}/attendance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AttendanceResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetChildAttendance_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync($"api/parents/children/{Guid.NewGuid()}/attendance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
