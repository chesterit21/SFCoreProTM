using AutoMapper;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.DTOs.States;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Mapping.Profiles;

public sealed class DomainToDtoMappingProfile : Profile
{
    public DomainToDtoMappingProfile()
    {
        CreateMap<Project, ProjectDto>();

    }
}
