using System.Linq;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.DTOs.States;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Mapping.Profiles;

public sealed class DomainToDtoMappingProfile : Profile
{
    public DomainToDtoMappingProfile()
    {
        CreateMap<Issue, IssueDto>()
            .ForMember(d => d.StartDate, opt => opt.MapFrom(s => s.Schedule.Start))
            .ForMember(d => d.TargetDate, opt => opt.MapFrom(s => s.Schedule.End))
            .ForMember(d => d.ExternalSource, opt => opt.MapFrom(s => s.ExternalReference == null ? null : s.ExternalReference.Source))
            .ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.ExternalReference == null ? null : s.ExternalReference.Identifier))
            .ForMember(d => d.AssigneeIds, opt => opt.MapFrom(s => s.AssigneeIds.ToArray()))
            .ForMember(d => d.LabelIds, opt => opt.MapFrom(s => s.LabelIds.ToArray()));

        CreateMap<IssueComment, IssueCommentDto>()
            .ForMember(d => d.CommentPlainText, opt => opt.MapFrom(s => s.Comment.PlainText))
            .ForMember(d => d.CommentHtml, opt => opt.MapFrom(s => s.Comment.Html))
            .ForMember(d => d.CommentJson, opt => opt.MapFrom(s => s.Comment.Json))
            .ForMember(d => d.Attachments, opt => opt.MapFrom(s => s.Attachments.Select(u => u.Value).ToArray()))
            .ForMember(d => d.ExternalSource, opt => opt.MapFrom(s => s.ExternalReference == null ? null : s.ExternalReference.Source))
            .ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.ExternalReference == null ? null : s.ExternalReference.Identifier));

        CreateMap<Project, ProjectDto>();

        CreateMap<State, StateDto>()
            .ForMember(d => d.ColorHex, opt => opt.MapFrom(s => s.Color.Value))
            .ForMember(d => d.Slug, opt => opt.MapFrom(s => s.Slug.Value))
            .ForMember(d => d.ExternalSource, opt => opt.MapFrom(s => s.ExternalReference == null ? null : s.ExternalReference.Source))
            .ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.ExternalReference == null ? null : s.ExternalReference.Identifier));
    }
}
