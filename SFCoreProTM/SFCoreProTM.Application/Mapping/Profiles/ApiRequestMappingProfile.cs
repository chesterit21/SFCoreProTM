using AutoMapper;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Mapping.Requests.Projects;

namespace SFCoreProTM.Application.Mapping.Profiles;

public class ApiRequestMappingProfile : Profile
{
    public ApiRequestMappingProfile()
    {
        CreateMap<CreateProjectRequest, CreateProjectRequestDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DescriptionPlainText ?? string.Empty));
            
        CreateMap<UpdateProjectRequest, UpdateProjectRequestDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DescriptionPlainText ?? string.Empty));
    }
}