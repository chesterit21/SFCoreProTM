using System;
using MediatR;

namespace SFCoreProTM.Application.Features.Modules.Commands.DeleteModule;

public class DeleteModuleCommand : IRequest<bool>
{
    public Guid ActorId { get; set; }
    public Guid ModuleId { get; set; }
}