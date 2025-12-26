using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Subjects.GetAll;

internal sealed class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, Result<List<SubjectResponse>>>
{
    private readonly ISubjectRepository _subjectRepository;

    public GetAllSubjectsQueryHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<Result<List<SubjectResponse>>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        var subjects = await _subjectRepository.GetAllAsync(cancellationToken);

        var response = subjects.Select(s => new SubjectResponse(s.Id, s.Name)).ToList();

        return response;
    }
}
