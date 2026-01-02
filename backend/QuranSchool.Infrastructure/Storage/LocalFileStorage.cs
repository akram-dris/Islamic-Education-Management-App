using QuranSchool.Application.Abstractions.Storage;

namespace QuranSchool.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private const string UploadsFolder = "uploads";

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), UploadsFolder);
        
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream, cancellationToken);

        return $"/files/{uniqueFileName}";
    }
}
