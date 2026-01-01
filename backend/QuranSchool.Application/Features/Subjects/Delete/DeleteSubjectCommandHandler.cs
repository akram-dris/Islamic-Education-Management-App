using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Caching;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Subjects.Delete;

public sealed class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, Result>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMemoryCache _cache;

    public DeleteSubjectCommandHandler(ISubjectRepository subjectRepository, IMemoryCache cache)
    {
        _subjectRepository = subjectRepository;
        _cache = cache;
    }

    public async Task<Result> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);

        if (subject is null)
        {
            return Result.Failure(DomainErrors.Subject.NotFound);
        }

        await _subjectRepository.DeleteAsync(subject, cancellationToken);

        _cache.Remove(CacheKeys.Subjects);

        return Result.Success();
    }
}
