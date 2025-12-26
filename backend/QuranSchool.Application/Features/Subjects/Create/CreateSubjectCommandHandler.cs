using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuranSchool.Application.Abstractions.Caching;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Subjects.Create;

public sealed class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Result<Guid>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMemoryCache _cache;

    public CreateSubjectCommandHandler(ISubjectRepository subjectRepository, IMemoryCache cache)
    {
        _subjectRepository = subjectRepository;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        if (await _subjectRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result<Guid>.Failure(DomainErrors.Subject.DuplicateName);
        }

        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _subjectRepository.AddAsync(subject, cancellationToken);

        _cache.Remove(CacheKeys.Subjects);

        return subject.Id;
    }
}