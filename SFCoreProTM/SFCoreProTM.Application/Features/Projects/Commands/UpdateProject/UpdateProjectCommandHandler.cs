using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {request.ProjectId} not found.");
        }

        // Update project details
        project.UpdateDetails(
            request.Payload.Name.Trim(),
            request.Payload.Description,
            request.Payload.ProjectPath,
            request.Payload.Status);

        // Persist changes
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        _projectRepository.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // Map to DTO and return
        return _mapper.Map<ProjectDto>(project);
    }
}