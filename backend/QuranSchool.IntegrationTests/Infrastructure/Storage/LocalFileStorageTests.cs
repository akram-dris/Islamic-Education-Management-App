using QuranSchool.Infrastructure.Storage;
using Xunit;
using FluentAssertions;
using System.Text;

namespace QuranSchool.IntegrationTests.Infrastructure.Storage;

public class LocalFileStorageTests : IDisposable
{
    private readonly LocalFileStorage _storage;
    private readonly string _uploadsPath;

    public LocalFileStorageTests()
    {
        _storage = new LocalFileStorage();
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    }

    [Fact]
    public async Task SaveFileAsync_ShouldSaveFile_WhenValidStreamProvided()
    {
        // Arrange
        const string fileName = "test.txt";
        const string content = "Hello World";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _storage.SaveFileAsync(stream, fileName);

        // Assert
        result.Should().StartWith("/files/");
        var savedFileName = result.Replace("/files/", "");
        var filePath = Path.Combine(_uploadsPath, savedFileName);
        
        File.Exists(filePath).Should().BeTrue();
        var savedContent = await File.ReadAllTextAsync(filePath);
        savedContent.Should().Be(content);
    }

    [Fact]
    public async Task SaveFileAsync_ShouldCreateDirectory_WhenItDoesNotExist()
    {
        // Arrange
        if (Directory.Exists(_uploadsPath))
        {
            Directory.Delete(_uploadsPath, true);
        }
        
        const string fileName = "newdir.txt";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("New Dir Content"));

        // Act
        await _storage.SaveFileAsync(stream, fileName);

        // Assert
        Directory.Exists(_uploadsPath).Should().BeTrue();
    }

    public void Dispose()
    {
        if (Directory.Exists(_uploadsPath))
        {
            var directory = new DirectoryInfo(_uploadsPath);
            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }
            // Directory.Delete(_uploadsPath); // Keep the directory to avoid issues with other tests if any
        }
    }
}
