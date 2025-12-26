using MediatR;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Features.Users.GetAll;

public record UserResponse(Guid Id, string Username, string FullName, string Role);

public record GetUsersQuery(UserRole Role) : IRequest<Result<List<UserResponse>>>;
