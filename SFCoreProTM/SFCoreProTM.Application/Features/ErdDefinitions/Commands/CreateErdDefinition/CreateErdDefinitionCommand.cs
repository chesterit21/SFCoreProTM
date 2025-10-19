using MediatR;
using SFCoreProTM.Application.DTOs.ErdDefinitions;

namespace SFCoreProTM.Application.Features.ErdDefinitions.Commands.CreateErdDefinition;

public class CreateErdDefinitionCommand : IRequest<ErdDefinitionDto>
{
    public CreateErdDefinitionRequestDto Request { get; set; } = new();
}