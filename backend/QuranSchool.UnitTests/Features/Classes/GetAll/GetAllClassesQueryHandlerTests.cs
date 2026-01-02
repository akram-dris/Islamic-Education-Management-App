using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Classes.GetAll;
using QuranSchool.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Classes.GetAll;

public class GetAllClassesQueryHandlerTests
{
    private readonly IClassRepository _classRepositoryMock;
    private readonly IMemoryCache _cacheMock;
    private readonly GetAllClassesQueryHandler _handler;

    public GetAllClassesQueryHandlerTests()
    {
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _cacheMock = Substitute.For<IMemoryCache>();
        _handler = new GetAllClassesQueryHandler(_classRepositoryMock, _cacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllClasses()
    {
        // Arrange
        var cls = Class.Create("Class A").Value;
        _classRepositoryMock.GetAllAsync(default).Returns(new List<Class> { cls });

        // Act
        var result = await _handler.Handle(new GetAllClassesQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Name.Should().Be("Class A");
    }
}
