using System.Net;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;
using FluentAssertions;

namespace QuranSchool.IntegrationTests.Features;

public class ProgramTests : FunctionalTest
{
    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await HttpClient.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task OpenApi_ShouldReturnOk()
    {
        // Act
        var response = await HttpClient.GetAsync("/openapi/v1.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Scalar_ShouldReturnOk()
    {
        // Act
        var response = await HttpClient.GetAsync("/scalar/v1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
