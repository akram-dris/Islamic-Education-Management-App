using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Subjects.GetAll;

public record GetAllSubjectsQuery() : IRequest<Result<List<SubjectResponse>>>;

public record SubjectResponse(Guid Id, string Name);
