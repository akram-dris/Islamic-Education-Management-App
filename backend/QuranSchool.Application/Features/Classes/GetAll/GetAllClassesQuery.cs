using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Classes.GetAll;

public record GetAllClassesQuery() : IRequest<Result<List<ClassResponse>>>;

public record ClassResponse(Guid Id, string Name);
