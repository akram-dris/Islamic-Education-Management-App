using MediatR;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Features.Users.GetAll;

public record UserResponse(Guid Id, string Username, string FullName, string Role);

public record GetUsersQuery(
    UserRole? Role = null, 
    string? SearchTerm = null, 
    int Page = 1, 
    int PageSize = 10) : IRequest<Result<PagedList<UserResponse>>>;
