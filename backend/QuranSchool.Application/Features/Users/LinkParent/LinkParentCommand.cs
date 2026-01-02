using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Users.LinkParent;

public record LinkParentCommand(Guid ParentId, Guid StudentId) : IRequest<Result>;
