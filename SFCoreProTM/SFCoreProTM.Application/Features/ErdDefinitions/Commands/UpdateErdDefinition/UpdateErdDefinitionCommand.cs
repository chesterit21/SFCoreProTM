using System;
using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Commands.UpdateErdDefinition;

public class UpdateErdDefinitionCommand : IRequest<ErdDefinitionDto>
{
    public Guid ErdDefinitionId { get; set; }
    public UpdateErdDefinitionRequestDto Request { get; set; } = new();
}