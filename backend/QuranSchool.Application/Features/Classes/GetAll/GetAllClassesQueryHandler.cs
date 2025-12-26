using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Classes.GetAll;

public sealed class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, Result<List<ClassResponse>>>
{
    private readonly IClassRepository _classRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "classes";

    public GetAllClassesQueryHandler(IClassRepository classRepository, IMemoryCache cache)
    {
        _classRepository = classRepository;
        _cache = cache;
    }

    public async Task<Result<List<ClassResponse>>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey, out List<ClassResponse>? response) && response is not null)
        {
            return response;
        }

        var classes = await _classRepository.GetAllAsync(cancellationToken);

        response = classes.Select(c => new ClassResponse(c.Id, c.Name)).ToList();

        _cache.Set(CacheKey, response, TimeSpan.FromMinutes(30));

        return response;
    }
}
