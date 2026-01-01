using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Caching;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Classes.Delete;

public sealed class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand, Result>
{
    private readonly IClassRepository _classRepository;
    private readonly IMemoryCache _cache;

    public DeleteClassCommandHandler(IClassRepository classRepository, IMemoryCache cache)
    {
        _classRepository = classRepository;
        _cache = cache;
    }

    public async Task<Result> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        var @class = await _classRepository.GetByIdAsync(request.ClassId, cancellationToken);

        if (@class is null)
        {
            return Result.Failure(DomainErrors.Class.NotFound);
        }

        // Soft delete is handled by the Interceptor/DbContext when Remove is called
        await _classRepository.DeleteAsync(@class, cancellationToken);

        _cache.Remove(CacheKeys.Classes);

        return Result.Success();
    }
}
