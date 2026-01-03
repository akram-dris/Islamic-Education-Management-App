using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using QuranSchool.API.Abstractions;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Features.Security;

public class ClaimsTests : FunctionalTest
{
    private void SetAuthToken(string token)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetMyChildren_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        // Arrange
        var token = JwtTestHelper.GenerateToken(new[]
        {
            new Claim(ClaimTypes.Role, RoleNames.Parent)
        });
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await HttpClient.GetAsync("api/parents/children");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyChildren_ShouldReturnUnauthorized_WhenClaimIsMissing()
    {
        // Arrange: Valid token but NO NameIdentifier claim
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.Role, "Parent") 
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.GetAsync("api/parents/children");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyChildren_ShouldReturnUnauthorized_WhenClaimIsInvalidGuid()
    {
        // Arrange: Valid token but invalid GUID in NameIdentifier
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid"),
            new Claim(ClaimTypes.Role, "Parent")
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.GetAsync("api/parents/children");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyAssignments_ShouldReturnUnauthorized_WhenClaimIsMissing()
    {
        // Arrange: Valid token but NO NameIdentifier claim
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.Role, "Student") 
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.GetAsync("api/assignments/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyAssignments_ShouldReturnUnauthorized_WhenClaimIsInvalidGuid()
    {
        // Arrange
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid"),
            new Claim(ClaimTypes.Role, "Student")
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.GetAsync("api/assignments/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SubmitSubmission_ShouldReturnUnauthorized_WhenClaimIsMissing()
    {
        // Arrange
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.Role, "Student") 
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/submissions", new { AssignmentId = Guid.NewGuid(), FileUrl = "url" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SubmitSubmission_ShouldReturnUnauthorized_WhenClaimIsInvalidGuid()
    {
        // Arrange
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid"),
            new Claim(ClaimTypes.Role, "Student")
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/submissions", new { AssignmentId = Guid.NewGuid(), FileUrl = "url" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyAttendance_ShouldReturnUnauthorized_WhenClaimIsMissing()
    {
        // Arrange
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.Role, "Student") 
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.GetAsync("api/attendance/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyAttendance_ShouldReturnUnauthorized_WhenClaimIsInvalidGuid()
    {
        // Arrange
        var token = JwtTestHelper.GenerateToken(new[] 
        { 
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid"),
            new Claim(ClaimTypes.Role, "Student")
        });
        SetAuthToken(token);

        // Act
        var response = await HttpClient.GetAsync("api/attendance/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
