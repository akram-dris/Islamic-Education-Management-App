namespace QuranSchool.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
