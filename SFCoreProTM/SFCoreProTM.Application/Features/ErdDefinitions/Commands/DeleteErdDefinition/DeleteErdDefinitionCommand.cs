using System;
using MediatR;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Commands.DeleteErdDefinition;

public class DeleteErdDefinitionCommand : IRequest<Unit>
{
    public Guid ErdDefinitionId { get; set; }
}