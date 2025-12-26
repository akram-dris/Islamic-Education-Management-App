using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.API.Abstractions;
using QuranSchool.Application.Abstractions.Storage;

namespace QuranSchool.API.Controllers;

[Authorize]
[Route(ApiRoutes.Files.BaseRoute)]
public class FilesController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    public FilesController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost(ApiRoutes.Files.Upload)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var stream = file.OpenReadStream();
        var fileUrl = await _fileStorage.SaveFileAsync(stream, file.FileName, cancellationToken);

        return Ok(new { fileUrl });
    }
}
