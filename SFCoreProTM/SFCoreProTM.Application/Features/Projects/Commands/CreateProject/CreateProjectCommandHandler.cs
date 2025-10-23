using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Shared;

namespace SFCoreProTM.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var projectName = payload.Name.Trim();

        // Validasi nama proyek
        if (await _projectRepository.NameExistsAsync(request.WorkspaceId, projectName, cancellationToken))
        {
            throw new ConflictException($"Project name '{projectName}' is already in use.");
        }

        // 1. Create the Aggregate Root
        var project = Project.Create(
            Guid.NewGuid(),
            request.WorkspaceId,
            projectName,
            payload.Description,
            payload.ProjectPath);

        // 2. Persist the aggregate
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        _projectRepository.Add(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // 3. Map to DTO and return
        return _mapper.Map<ProjectDto>(project);
    }
}