using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Caching;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Subjects.Update;

public sealed class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, Result>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMemoryCache _cache;

    public UpdateSubjectCommandHandler(ISubjectRepository subjectRepository, IMemoryCache cache)
    {
        _subjectRepository = subjectRepository;
        _cache = cache;
    }

    public async Task<Result> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);

        if (subject is null)
        {
            return Result.Failure(DomainErrors.Subject.NotFound);
        }

        if (request.Name != subject.Name && await _subjectRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result.Failure(DomainErrors.Subject.DuplicateName);
        }

        var result = subject.Update(request.Name);
        if (result.IsFailure)
        {
            return result;
        }

        await _subjectRepository.UpdateAsync(subject, cancellationToken);

        _cache.Remove(CacheKeys.Subjects);

        return Result.Success();
    }
}
