using MediatR;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Parents.GetChildren;

public record GetChildrenQuery(Guid ParentId) : IRequest<Result<List<UserResponse>>>;
