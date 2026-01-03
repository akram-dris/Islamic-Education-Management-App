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
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Application.Features.Attendance.GetMy;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Attendance;

public class AttendanceControllerTests : FunctionalTest
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
    public async Task Mark_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PostAsJsonAsync("api/attendance/mark", new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/attendance/{Guid.NewGuid()}", new UpdateAttendanceRequest(DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.DeleteAsync($"api/attendance/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Mark_ShouldReturnForbidden_WhenUserIsStudent()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_att_fob", "Pass123!", "S", UserRole.Student));

        SetAuthToken(await AuthenticateAsync("s_att_fob", "Pass123!"));

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/attendance/mark", new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Mark_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var tId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_att", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var sId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_att", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var cId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Att"))).Content.ReadFromJsonAsync<Guid>();
        var subId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Att"))).Content.ReadFromJsonAsync<Guid>();
        var aId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(tId, cId, subId))).Content.ReadFromJsonAsync<Guid>();

        var teacherToken = await AuthenticateAsync("t_att", "Pass123!");
        SetAuthToken(teacherToken);

        var command = new MarkAttendanceCommand(
            aId, 
            DateOnly.FromDateTime(DateTime.Now), 
            new List<AttendanceRecordDto> { new(sId, AttendanceStatus.Present) });

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/attendance/mark", command);

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var detail = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed with {response.StatusCode}: {detail}");
        }
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var tId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_att_upd", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var sId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_att_upd", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var cId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Att_Upd"))).Content.ReadFromJsonAsync<Guid>();
        var subId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Att_Upd"))).Content.ReadFromJsonAsync<Guid>();
        var aId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(tId, cId, subId))).Content.ReadFromJsonAsync<Guid>();

        var teacherToken = await AuthenticateAsync("t_att_upd", "Pass123!");
        SetAuthToken(teacherToken);

        var createResp = await HttpClient.PostAsJsonAsync("api/attendance/mark", new MarkAttendanceCommand(aId, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto> { new(sId, AttendanceStatus.Present) }));
        if (createResp.StatusCode != HttpStatusCode.OK)
        {
            var detail = await createResp.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with {createResp.StatusCode}: {detail}");
        }
        var sessionId = await createResp.Content.ReadFromJsonAsync<Guid>();

        var request = new UpdateAttendanceRequest(
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            new List<AttendanceRecordDto> { new(sId, AttendanceStatus.Absent) });

        // Act
        var response = await HttpClient.PutAsJsonAsync($"api/attendance/{sessionId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMy_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.GetAsync("api/attendance/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMy_ShouldReturnAttendance_WhenAuthenticatedAsStudent()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_att_my", "Pass123!", "S", UserRole.Student));

        var studentToken = await AuthenticateAsync("s_att_my", "Pass123!");
        SetAuthToken(studentToken);

        // Act
        var response = await HttpClient.GetAsync("api/attendance/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AttendanceResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSessionExists()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        
        var tId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_att_del", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var sId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_att_del", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var cId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Att_Del"))).Content.ReadFromJsonAsync<Guid>();
        var subId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Att_Del"))).Content.ReadFromJsonAsync<Guid>();
        var aId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(tId, cId, subId))).Content.ReadFromJsonAsync<Guid>();
        
        var teacherToken = await AuthenticateAsync("t_att_del", "Pass123!");
        SetAuthToken(teacherToken);

        var createResp = await HttpClient.PostAsJsonAsync("api/attendance/mark", new MarkAttendanceCommand(aId, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto> { new(sId, AttendanceStatus.Present) }));
        if (createResp.StatusCode != HttpStatusCode.OK)
        {
            var detail = await createResp.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with {createResp.StatusCode}: {detail}");
        }
        var sessionId = await createResp.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/attendance/{sessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}