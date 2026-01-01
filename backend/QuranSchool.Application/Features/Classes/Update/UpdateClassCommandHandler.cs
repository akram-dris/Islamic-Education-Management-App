using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Caching;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Classes.Update;

public sealed class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, Result>
{
    private readonly IClassRepository _classRepository;
    private readonly IMemoryCache _cache;

    public UpdateClassCommandHandler(IClassRepository classRepository, IMemoryCache cache)
    {
        _classRepository = classRepository;
        _cache = cache;
    }

    public async Task<Result> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var @class = await _classRepository.GetByIdAsync(request.ClassId, cancellationToken);

        if (@class is null)
        {
            return Result.Failure(DomainErrors.Class.NotFound);
        }

        if (request.Name != @class.Name && await _classRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result.Failure(DomainErrors.Class.DuplicateName);
        }

        @class.Name = request.Name;

        await _classRepository.UpdateAsync(@class, cancellationToken);

        _cache.Remove(CacheKeys.Classes);

        return Result.Success();
    }
}
