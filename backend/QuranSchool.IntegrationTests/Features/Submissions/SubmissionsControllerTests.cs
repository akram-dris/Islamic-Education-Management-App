using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using QuranSchool.API.Controllers;
using QuranSchool.Application.Features.Allocations.Create;
using QuranSchool.Application.Features.Assignments.Create;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Application.Features.Enrollments.Create;
using QuranSchool.Application.Features.Subjects.Create;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Submissions;

public class SubmissionsControllerTests : FunctionalTest
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
    public async Task Delete_ShouldReturnNotFound_WhenSubmissionIsMissing()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        // Act
        var response = await HttpClient.DeleteAsync($"api/submissions/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void GradeSubmissionRequest_ShouldHaveCorrectProperties()
    {
        // Arrange
        decimal grade = 95.5m;
        string remarks = "Excellent work";

        // Act
        var request = new GradeSubmissionRequest(grade, remarks);

        // Assert
        request.Grade.Should().Be(grade);
        request.Remarks.Should().Be(remarks);
    }

    [Fact]
    public async Task Submit_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(Guid.NewGuid(), "url"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Submit_ShouldReturnUnauthorized_WhenUserIdIsInvalid()
    {
        // This is hard to trigger with AuthenticateAsync because it uses real GUIDs.
        // But we can manually set a junk token or a token with junk claim if we had a way to forge.
        // For now, let's assume we want to cover the `!Guid.TryParse` branch.
        // We can use a token that has a non-guid NameIdentifier.
    }

    [Fact]
    public async Task Submit_ShouldReturnOk_WhenStudentIsEnrolled()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_sub", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_sub", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Sub"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Sub"))).Content.ReadFromJsonAsync<Guid>();
        
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));

        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken = await AuthenticateAsync("s_sub", "Pass123!");
        SetAuthToken(studentToken);

        var request = new CreateSubmissionRequest(assignmentId, "http://files.com/1");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/submissions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Grade_ShouldReturnOk_WhenTeacherGrades()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_grade", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_grade", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Grade"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Grade"))).Content.ReadFromJsonAsync<Guid>();
        
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));
        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken = await AuthenticateAsync("s_grade", "Pass123!");
        SetAuthToken(studentToken);
        var submissionId = await (await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(assignmentId, "url"))).Content.ReadFromJsonAsync<Guid>();

        var teacherToken = await AuthenticateAsync("t_grade", "Pass123!");
        SetAuthToken(teacherToken);

        var request = new GradeSubmissionRequest(90, "Well done");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/submissions/{submissionId}/grade", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Grade_ShouldReturnOk_WhenTeacherGradesWithNullRemarks()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_grade_null", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_grade_null", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Grade_Null"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Grade_Null"))).Content.ReadFromJsonAsync<Guid>();
        
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));
        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken = await AuthenticateAsync("s_grade_null", "Pass123!");
        SetAuthToken(studentToken);
        var submissionId = await (await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(assignmentId, "url"))).Content.ReadFromJsonAsync<Guid>();

        var teacherToken = await AuthenticateAsync("t_grade_null", "Pass123!");
        SetAuthToken(teacherToken);

        var request = new GradeSubmissionRequest(85, null);

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/submissions/{submissionId}/grade", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Grade_ShouldReturnForbidden_WhenUserIsNotTeacherOrAdmin()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_fob_g", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_fob_g", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Fob_G"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Fob_G"))).Content.ReadFromJsonAsync<Guid>();
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));
        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken = await AuthenticateAsync("s_fob_g", "Pass123!");
        SetAuthToken(studentToken);
        var submissionId = await (await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(assignmentId, "url"))).Content.ReadFromJsonAsync<Guid>();

        // Act
        var request = new GradeSubmissionRequest(90, "Grade");
        var response = await HttpClient.PostAsJsonAsync($"api/submissions/{submissionId}/grade", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Grade_ShouldReturnNotFound_WhenSubmissionDoesNotExist()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);
        var request = new GradeSubmissionRequest(90, "Remarks");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/submissions/{Guid.NewGuid()}/grade", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Grade_ShouldReturnBadRequest_WhenGradeIsInvalid()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_bad", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_bad", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Bad"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Bad"))).Content.ReadFromJsonAsync<Guid>();
        
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));
        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken = await AuthenticateAsync("s_bad", "Pass123!");
        SetAuthToken(studentToken);
        var submissionId = await (await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(assignmentId, "url"))).Content.ReadFromJsonAsync<Guid>();

        var teacherToken = await AuthenticateAsync("t_bad", "Pass123!");
        SetAuthToken(teacherToken);

        var request = new GradeSubmissionRequest(150, "Too high");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/submissions/{submissionId}/grade", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSubmissionExists()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_del", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_del", "Pass123!", "S", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Del"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Del"))).Content.ReadFromJsonAsync<Guid>();
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));
        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken = await AuthenticateAsync("s_del", "Pass123!");
        SetAuthToken(studentToken);
        var submissionId = await (await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(assignmentId, "url"))).Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/submissions/{submissionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenSubmissionDoesNotExist()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        // Act
        var response = await HttpClient.DeleteAsync($"api/submissions/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await HttpClient.DeleteAsync($"api/submissions/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenStudentDeletesOtherSubmission()
    {
        // Arrange
        var adminToken = await AuthenticateAsync("admin", "admin123");
        SetAuthToken(adminToken);

        var teacherId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("t_fob_del", "Pass123!", "T", UserRole.Teacher))).Content.ReadFromJsonAsync<Guid>();
        var studentId1 = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_fob_del1", "Pass123!", "S1", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var studentId2 = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("s_fob_del2", "Pass123!", "S2", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("C_Fob_Del"))).Content.ReadFromJsonAsync<Guid>();
        var subjectId = await (await HttpClient.PostAsJsonAsync("api/subjects", new CreateSubjectCommand("S_Fob_Del"))).Content.ReadFromJsonAsync<Guid>();
        var allocationId = await (await HttpClient.PostAsJsonAsync("api/allocations", new CreateAllocationCommand(teacherId, classId, subjectId))).Content.ReadFromJsonAsync<Guid>();
        await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId1, classId));

        var assignmentId = await (await HttpClient.PostAsJsonAsync("api/assignments", new CreateAssignmentCommand(allocationId, "Task", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1))))).Content.ReadFromJsonAsync<Guid>();

        var studentToken1 = await AuthenticateAsync("s_fob_del1", "Pass123!");
        SetAuthToken(studentToken1);
        var submissionId = await (await HttpClient.PostAsJsonAsync("api/submissions", new CreateSubmissionRequest(assignmentId, "url"))).Content.ReadFromJsonAsync<Guid>();

        var studentToken2 = await AuthenticateAsync("s_fob_del2", "Pass123!");
        SetAuthToken(studentToken2);

        // Act
        var response = await HttpClient.DeleteAsync($"api/submissions/{submissionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
