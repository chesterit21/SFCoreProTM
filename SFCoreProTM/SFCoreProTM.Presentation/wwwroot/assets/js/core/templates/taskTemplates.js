
import { escapeHtml, truncateText } from '../utils/domUtils.js';

function resolveTaskIsErd(task) {
    return Boolean(task?.isErd ?? task?.IsErd);
}

export function renderTaskTable(tasksContainer, tasks, moduleId) {
    const tableRows = tasks.map(task => {
        const taskId = task.id ?? task.Id;
        const isErd = resolveTaskIsErd(task);
        return `
            <tr>
                <td>${escapeHtml(task.name)}</td>
                <td><span class="badge bg-${isErd ? 'success' : 'danger'}">${isErd ? 'ERD' : 'Non-ERD'}</span></td>
                <td>${escapeHtml(truncateText(task.description || '', 70))}</td>
                <td>${task.sortOrder}</td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-info-sm view-task-btn" data-task-id="${taskId}" title="View Task">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-outline-primary-sm edit-task-btn" data-task-id="${taskId}" title="Edit Task">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-outline-danger-sm delete-task-btn" data-task-id="${taskId}" title="Delete Task">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');

    const tableHtml = `
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h6>Flow Tasks</h6>
            <button class="btn btn-outline-success-sm create-task-btn" data-module-id="${moduleId}" title="New Task">
                <i class="fas fa-plus"></i>
            </button>
        </div>
        <div class="table-responsive">
            <table class="table table-striped table-hover table-sm">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Description</th>
                        <th>Sort Order</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${tableRows}
                </tbody>
            </table>
        </div>
        ${!tasks || tasks.length === 0 ? '<p class="text-center text-muted">No tasks found.</p>' : ''}
    `;

    tasksContainer.innerHTML = tableHtml;
}

export function renderTaskDetailPlaceholder() {
    const detailContainer = document.getElementById('detail-of-task');
    if (detailContainer && !detailContainer.innerHTML.trim()) {
        detailContainer.innerHTML = `
            <div class="alert alert-info shadow-sm" role="alert">
                Select a task from the list to view its details.
            </div>
        `;
    }
}

export function renderTaskDetail(task, context) {
    const detailContainer = document.getElementById('detail-of-task');
    if (!detailContainer) return;

    const isErdTask = resolveTaskIsErd(task);
    const { taskId, moduleId, projectId, workspaceId } = context;

    const accordionId = `taskDetailAccordion-${taskId}`;
    const flowCollapseId = `task-detail-flow-${taskId}`;
    const erdCollapseId = `task-detail-erd-${taskId}`;
    const flowContainerId = `flow-task-detail-${taskId}`;
    const erdFormWrapperId = `erd-definition-form-wrapper-${moduleId}-${taskId}`;

    const descriptionHtml = task?.description
        ? `<p class="mb-0">${escapeHtml(task.description)}</p>`
        : '<p class="mb-0 text-muted">No description provided.</p>';

    detailContainer.dataset.currentTaskId = taskId;
    if (moduleId) {
        detailContainer.dataset.currentModuleId = moduleId;
    }

    detailContainer.innerHTML = `
        <div class="card shadow-sm mb-3">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start gap-3">
                    <div>
                        <h5 class="mb-1">${escapeHtml(task?.name ?? 'No name')}</h5>
                        ${descriptionHtml}
                    </div>
                    <span class="badge bg-${isErdTask ? 'success' : 'secondary'}">${isErdTask ? 'ERD Task' : 'Non-ERD Task'}</span>
                    <button class="btn btn-outline-secondary-sm accordion-action-btn add-flow-task-btn" 
                        data-task-id="${taskId}"
                        data-module-id="${moduleId ?? ''}"
                        data-project-id="${projectId ?? ''}"
                        data-workspace-id="${workspaceId ?? ''}" title="Add Flow Task">
                        <i class="fas fa-plus"></i>
                    </button>
                </div>
            </div>
        </div>
        <div class="accordion accordion-flush" id="${accordionId}">
            <div class="accordion-item border rounded-3 shadow-sm mb-3">
                <h2 class="accordion-header position-relative" id="heading-flow-${taskId}">
                    <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#${flowCollapseId}" aria-expanded="true" aria-controls="${flowCollapseId}">
                        List of Flow Tasks
                    </button>
                </h2>
                <div id="${flowCollapseId}" class="accordion-collapse collapse show" data-bs-parent="#${accordionId}">
                    <div class="accordion-body">
                        <div class="flow-tasks-container"
                             id="${flowContainerId}"
                             data-task-id="${taskId}"
                             data-module-id="${moduleId ?? ''}"
                             data-project-id="${projectId ?? ''}"
                             data-workspace-id="${workspaceId ?? ''}">
                            <p class="text-muted small mb-0">Loading flow tasks...</p>
                        </div>
                    </div>
                </div>
            </div>
            <div class="accordion-item border rounded-3 shadow-sm">
                <h2 class="accordion-header" id="heading-erd-${taskId}">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#${erdCollapseId}" aria-expanded="false" aria-controls="${erdCollapseId}">
                        List of ERD Definitions
                    </button>
                </h2>
                <div id="${erdCollapseId}" class="accordion-collapse collapse" data-bs-parent="#${accordionId}">
                    <div class="accordion-body" id="${erdFormWrapperId}">
                        <!-- ERD Form and List will be rendered here by erdEventHandlers.js -->
                    </div>
                </div>
            </div>
        </div>
    `;
}
