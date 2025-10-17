using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Labels;

namespace SFCoreProTM.Application.Features.Labels.Commands.CreateLabel;

public sealed record CreateLabelCommand(Guid WorkspaceId, Guid ProjectId, Guid ActorId, CreateLabelRequestDto Payload) : IRequest<LabelDto>;
