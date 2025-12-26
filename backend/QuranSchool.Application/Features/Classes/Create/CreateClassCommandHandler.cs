using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Features.Classes.Create;

public sealed class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Result<Guid>>
{
    private readonly IClassRepository _classRepository;
    private readonly IMemoryCache _cache;

    public CreateClassCommandHandler(IClassRepository classRepository, IMemoryCache cache)
    {
        _classRepository = classRepository;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        if (await _classRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result<Guid>.Failure(Error.Conflict("Class.DuplicateName", "A class with this name already exists."));
        }

        var @class = new Class
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _classRepository.AddAsync(@class, cancellationToken);

        _cache.Remove("classes");

        return @class.Id;
    }
}
