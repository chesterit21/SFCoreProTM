using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Users;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IUserRepository _users;

    public GetCurrentUserQueryHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);
        return new CurrentUserDto
        {
            UserId = user?.Id ?? request.UserId,
            Email = user?.Email?.Value ?? string.Empty,
            DisplayName = user?.DisplayName ?? string.Empty,
            LastLoginAt = user?.LastLoginTime,
            IsPasswordAutoset = user?.IsPasswordAutoset ?? false,
        };
    }
}

