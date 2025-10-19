using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Modules;

namespace SFCoreProTM.Application.Features.Modules.Commands.UpdateModule;

public class UpdateModuleCommand : IRequest<ModuleDto>
{
    public Guid ActorId { get; set; }
    public Guid ModuleId { get; set; }
    public UpdateModuleRequestDto Request { get; set; } = new();
}