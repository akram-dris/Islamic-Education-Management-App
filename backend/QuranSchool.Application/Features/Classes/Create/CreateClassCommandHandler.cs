using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Features.Classes.Create;

internal sealed class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Result<Guid>>
{
    private readonly IClassRepository _classRepository;

    public CreateClassCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<Result<Guid>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var @class = new Class
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _classRepository.AddAsync(@class, cancellationToken);

        return @class.Id;
    }
}
