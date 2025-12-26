using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Features.Subjects.Create;

public sealed class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Result<Guid>>
{
    private readonly ISubjectRepository _subjectRepository;

    public CreateSubjectCommandHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<Result<Guid>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        if (await _subjectRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result<Guid>.Failure(Error.Conflict("Subject.DuplicateName", "A subject with this name already exists."));
        }

        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _subjectRepository.AddAsync(subject, cancellationToken);

        return subject.Id;
    }
}