using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Application.Features.Enrollments.Create;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Enums;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Enrollments;

public class EnrollmentsControllerTests : FunctionalTest
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
        
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("stu_en", "Pass123!", "Stu", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("Class En"))).Content.ReadFromJsonAsync<Guid>();

        var command = new CreateEnrollmentCommand(studentId, classId);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/enrollments", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenEnrollmentExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        
        var studentId = await (await HttpClient.PostAsJsonAsync("api/users/register", new RegisterUserCommand("stu_un", "Pass123!", "Stu", UserRole.Student))).Content.ReadFromJsonAsync<Guid>();
        var classId = await (await HttpClient.PostAsJsonAsync("api/classes", new CreateClassCommand("Class Un"))).Content.ReadFromJsonAsync<Guid>();

        var createResponse = await HttpClient.PostAsJsonAsync("api/enrollments", new CreateEnrollmentCommand(studentId, classId));
        var enrollmentId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await HttpClient.DeleteAsync($"api/enrollments/{enrollmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
