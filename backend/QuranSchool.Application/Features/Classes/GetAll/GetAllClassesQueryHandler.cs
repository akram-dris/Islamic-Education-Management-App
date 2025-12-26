using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Classes.GetAll;

public sealed class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, Result<List<ClassResponse>>>
{
    private readonly IClassRepository _classRepository;

    public GetAllClassesQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<Result<List<ClassResponse>>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
    {
        var classes = await _classRepository.GetAllAsync(cancellationToken);

        var response = classes.Select(c => new ClassResponse(c.Id, c.Name)).ToList();

        return response;
    }
}
