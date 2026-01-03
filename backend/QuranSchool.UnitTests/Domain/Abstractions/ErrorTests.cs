using FluentAssertions;
using QuranSchool.Domain.Abstractions;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Abstractions;

public class ErrorTests
{
    [Fact]
    public void FactoryMethods_ShouldSetCorrectType()
    {
        Error.NotFound("C", "D").Type.Should().Be(ErrorType.NotFound);
        Error.Validation("C", "D").Type.Should().Be(ErrorType.Validation);
        Error.Conflict("C", "D").Type.Should().Be(ErrorType.Conflict);
        Error.Failure("C", "D").Type.Should().Be(ErrorType.Failure);
        Error.Unauthorized("C", "D").Type.Should().Be(ErrorType.Unauthorized);
        Error.Forbidden("C", "D").Type.Should().Be(ErrorType.Forbidden);
    }
}
