using System;
using MediatR;
using SFCoreProTM.Application.DTOs.UserProfiles;

namespace SFCoreProTM.Application.Features.UserProfiles.Queries.GetByUserId;

public sealed record GetUserProfileByUserIdQuery(Guid UserId) : IRequest<UserProfileDto?>;