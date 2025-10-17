using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Mapping.Requests.Issues;
using SFCoreProTM.Application.Mapping.Requests.Projects;

namespace SFCoreProTM.Application.Mapping.Profiles;

public sealed class ApiRequestMappingProfile : Profile
{
    public ApiRequestMappingProfile()
    {
        CreateMap<CreateIssueRequest, CreateIssueRequestDto>();
        CreateMap<UpdateIssueRequest, UpdateIssueRequestDto>();
        CreateMap<CreateIssueCommentRequest, CreateIssueCommentRequestDto>();
        CreateMap<UpdateIssueCommentRequest, UpdateIssueCommentRequestDto>();

        CreateMap<CreateProjectRequest, CreateProjectRequestDto>()
            .ForMember(d => d.Timezone, opt => opt.MapFrom(s => string.IsNullOrWhiteSpace(s.Timezone) ? "UTC" : s.Timezone!));
    }
}

