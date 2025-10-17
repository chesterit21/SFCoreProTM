using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Users;

namespace SFCoreProTM.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<CurrentUserDto>;

