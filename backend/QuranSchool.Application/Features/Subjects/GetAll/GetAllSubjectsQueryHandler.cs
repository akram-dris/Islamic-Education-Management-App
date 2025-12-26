using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Subjects.GetAll;

public sealed class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, Result<List<SubjectResponse>>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "subjects";

    public GetAllSubjectsQueryHandler(ISubjectRepository subjectRepository, IMemoryCache cache)
    {
        _subjectRepository = subjectRepository;
        _cache = cache;
    }

    public async Task<Result<List<SubjectResponse>>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey, out List<SubjectResponse>? response) && response is not null)
        {
            return response;
        }

        var subjects = await _subjectRepository.GetAllAsync(cancellationToken);

        response = subjects.Select(s => new SubjectResponse(s.Id, s.Name)).ToList();

        _cache.Set(CacheKey, response, TimeSpan.FromMinutes(30));

        return response;
    }
}
