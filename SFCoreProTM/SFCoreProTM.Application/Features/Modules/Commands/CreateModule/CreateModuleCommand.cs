using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Modules;

namespace SFCoreProTM.Application.Features.Modules.Commands.CreateModule;

public class CreateModuleCommand : IRequest<ModuleDto>
{
    public Guid ActorId { get; set; }
    public CreateModuleRequestDto Request { get; set; } = new();
}