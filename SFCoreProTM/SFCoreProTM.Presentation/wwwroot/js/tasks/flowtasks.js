(() => {
    const flowTaskCache = new Map();

    const STATUS_LABELS = {
        0: 'In Progress',
        1: 'Done',
        2: 'Canceled',
        3: 'Pause'
    };

    const STATUS_BADGES = {
        0: 'info',
        1: 'success',
        2: 'danger',
        3: 'warning'
    };

function resolveContext(options) {
    if (!options) {
        return null;
    }

    const context = { ...options };

    if (typeof context.containerId === 'string') {
        context.container = document.getElementById(context.containerId);
        }

        if (!context.container && context.taskId) {
            context.container = document.querySelector(`.flow-tasks-container[data-task-id="${context.taskId}"]`);
            if (context.container) {
                context.containerId = context.container.id;
            }
        }

        if (!context.container) {
            console.error('Flow task container not found for context:', context);
            return null;
        }

        const { dataset } = context.container;
        context.taskId = context.taskId || dataset.taskId;
        context.moduleId = context.moduleId || dataset.moduleId;
        context.projectId = context.projectId || dataset.projectId;
        context.workspaceId = context.workspaceId || dataset.workspaceId;

        return context;
    }

    function buildRequestPayload(context, formData) {
        return {
            WorkspaceId: context.workspaceId,
            ProjectId: context.projectId,
            ModuleId: context.moduleId,
            TaskId: context.taskId,
            Name: formData.get('name') || '',
            Description: formData.get('description') || '',
            SortOrder: parseInt(formData.get('sortOrder'), 10) || 1
        };
    }

    function getStatusBadge(flowTask) {
        const badgeClass = STATUS_BADGES[flowTask.flowStatus] || 'secondary';
        const label = STATUS_LABELS[flowTask.flowStatus] || 'Unknown';
        return `<span class="badge bg-${badgeClass}">${label}</span>`;
    }

function renderFlowTasks(context, flowTasks) {
    if (!context) {
        return;
    }

    const container = context.container;
    if (!container) {
        return;
    }

    if (!flowTasks || flowTasks.length === 0) {
        container.innerHTML = '<div class="alert alert-info mb-0">No flow tasks yet.</div>';
        return;
    }

    let html = '<div class="card border-0 shadow-sm"><ul class="list-group list-group-flush">';
    flowTasks
        .slice()
        .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
        .forEach(flowTask => {
            const description = flowTask.description ? (window.truncateText ? window.truncateText(flowTask.description, 120) : flowTask.description) : 'No description';
            html += `
                <li class="list-group-item">
                    <div class="d-flex justify-content-between align-items-start gap-3">
                        <div class="me-auto">
                            <div class="d-flex align-items-center gap-2 mb-2">
                                <strong>${flowTask.name || 'Untitled Flow Task'}</strong>
                                ${getStatusBadge(flowTask)}
                            </div>
                            <div class="small text-muted mb-1">Sort Order: ${flowTask.sortOrder || 0}</div>
                            <div class="small text-muted">${description}</div>
                        </div>
                        <div class="btn-group btn-group-sm flex-shrink-0">
                            <button class="btn btn-outline-primary-sm edit-flow-task-btn"
                                    data-flow-task-id="${flowTask.id}"
                                    data-task-id="${flowTask.taskId}">
                                Edit
                            </button>
                            <button class="btn btn-outline-danger-sm delete-flow-task-btn"
                                    data-flow-task-id="${flowTask.id}"
                                    data-task-id="${flowTask.taskId}">
                                Delete
                            </button>
                        </div>
                    </div>
                </li>
            `;
        });
    html += '</ul></div>';

    container.innerHTML = html;

    container.querySelectorAll('.edit-flow-task-btn').forEach(button => {
        button.addEventListener('click', () => {
            openEditModal({
                containerId: container.id,
                taskId: button.dataset.taskId,
                flowTaskId: button.dataset.flowTaskId,
                moduleId: context.moduleId,
                projectId: context.projectId,
                workspaceId: context.workspaceId
            });
        });
    });

    container.querySelectorAll('.delete-flow-task-btn').forEach(button => {
        button.addEventListener('click', () => {
            deleteFlowTask({
                containerId: container.id,
                taskId: button.dataset.taskId,
                flowTaskId: button.dataset.flowTaskId,
                moduleId: context.moduleId,
                projectId: context.projectId,
                workspaceId: context.workspaceId
                });
            });
        });
    }

function refreshFlowTasks(options) {
    const baseContext = resolveContext(options);
    if (!baseContext || !baseContext.taskId) {
        return;
    }

    fetch(`/api/tasks/${baseContext.taskId}/flow-tasks`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(flowTasks => {
            flowTaskCache.set(baseContext.taskId, flowTasks);

            const containers = Array.from(document.querySelectorAll(`.flow-tasks-container[data-task-id="${baseContext.taskId}"]`));

            if (containers.length === 0) {
                if (baseContext.container) {
                    renderFlowTasks(baseContext, flowTasks);
                }
                return;
            }

            containers.forEach(container => {
                const derivedContext = {
                    ...baseContext,
                    container,
                    containerId: container.id || baseContext.containerId,
                    moduleId: container.dataset.moduleId || baseContext.moduleId,
                    projectId: container.dataset.projectId || baseContext.projectId,
                    workspaceId: container.dataset.workspaceId || baseContext.workspaceId
                };
                renderFlowTasks(derivedContext, flowTasks);
            });
        })
        .catch(error => {
            console.error('Error loading flow tasks:', error);
            const containers = Array.from(document.querySelectorAll(`.flow-tasks-container[data-task-id="${baseContext.taskId}"]`));
            if (containers.length > 0) {
                containers.forEach(container => {
                    container.innerHTML = '<div class="alert alert-danger mb-0">Unable to load flow tasks.</div>';
                });
            } else if (baseContext.container) {
                baseContext.container.innerHTML = '<div class="alert alert-danger mb-0">Unable to load flow tasks.</div>';
            }
            if (window.showNotification) {
                window.showNotification(`Error loading flow tasks: ${error.message}`, 'danger');
            }
        });
    }

    function wireCreateForm(modalElement, context) {
        const form = modalElement.querySelector('#createFlowTaskForm');
        if (!form) {
            return;
        }

        const workspaceInput = form.querySelector('#flowTaskWorkspaceId');
        const projectInput = form.querySelector('#flowTaskProjectId');
        const moduleInput = form.querySelector('#flowTaskModuleId');
        const taskInput = form.querySelector('#flowTaskTaskId');

        if (workspaceInput) workspaceInput.value = context.workspaceId || '';
        if (projectInput) projectInput.value = context.projectId || '';
        if (moduleInput) moduleInput.value = context.moduleId || '';
        if (taskInput) taskInput.value = context.taskId || '';

        form.addEventListener('submit', event => {
            event.preventDefault();
            const formData = new FormData(form);
            const payload = buildRequestPayload(context, formData);

            if (!payload.WorkspaceId || !payload.ProjectId || !payload.ModuleId || !payload.TaskId) {
                if (window.showNotification) {
                    window.showNotification('Missing workspace, project, or module information to create flow task.', 'danger');
                }
                return;
            }

            fetch(`/api/tasks/${context.taskId}/flow-tasks`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(text || 'Failed to create flow task');
                    });
                }
                return response.json();
            })
            .then(() => {
                const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
                modalInstance.hide();
                if (window.showNotification) {
                    window.showNotification('Flow task created successfully.', 'success');
                }
                refreshFlowTasks(context);
            })
            .catch(error => {
                console.error('Error creating flow task:', error);
                if (window.showNotification) {
                    window.showNotification(`Error creating flow task: ${error.message}`, 'danger');
                }
            });
        }, { once: true });
    }

    function wireEditForm(modalElement, context, flowTask) {
        const form = modalElement.querySelector('#editFlowTaskForm');
        if (!form) {
            return;
        }

        const workspaceInput = form.querySelector('#editFlowTaskWorkspaceId');
        const projectInput = form.querySelector('#editFlowTaskProjectId');
        const moduleInput = form.querySelector('#editFlowTaskModuleId');
        const taskInput = form.querySelector('#editFlowTaskTaskId');
        const flowTaskIdInput = form.querySelector('#editFlowTaskId');

        if (workspaceInput) workspaceInput.value = context.workspaceId || '';
        if (projectInput) projectInput.value = context.projectId || '';
        if (moduleInput) moduleInput.value = context.moduleId || '';
        if (taskInput) taskInput.value = context.taskId || '';
        if (flowTaskIdInput) flowTaskIdInput.value = context.flowTaskId || '';

        if (flowTask) {
            const nameInput = form.querySelector('#editFlowTaskName');
            const descriptionInput = form.querySelector('#editFlowTaskDescription');
            const sortOrderInput = form.querySelector('#editFlowTaskSortOrder');

            if (nameInput) nameInput.value = flowTask.name || '';
            if (descriptionInput) descriptionInput.value = flowTask.description || '';
            if (sortOrderInput) sortOrderInput.value = flowTask.sortOrder || 1;
        }

        form.addEventListener('submit', event => {
            event.preventDefault();
            const formData = new FormData(form);
            const payload = buildRequestPayload(context, formData);

            if (!payload.WorkspaceId || !payload.ProjectId || !payload.ModuleId || !payload.TaskId) {
                if (window.showNotification) {
                    window.showNotification('Missing workspace, project, or module information to update flow task.', 'danger');
                }
                return;
            }

            fetch(`/api/tasks/${context.taskId}/flow-tasks/${context.flowTaskId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(text || 'Failed to update flow task');
                    });
                }
                return response.json();
            })
            .then(() => {
                const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
                modalInstance.hide();
                if (window.showNotification) {
                    window.showNotification('Flow task updated successfully.', 'success');
                }
                refreshFlowTasks(context);
            })
            .catch(error => {
                console.error('Error updating flow task:', error);
                if (window.showNotification) {
                    window.showNotification(`Error updating flow task: ${error.message}`, 'danger');
                }
            });
        }, { once: true });
    }

    function openCreateModal(options) {
        const context = resolveContext(options);
        if (!context || !context.taskId) {
            if (window.showNotification) {
                window.showNotification('Cannot open flow task modal due to missing task information.', 'danger');
            }
            return;
        }

        const modalElement = document.getElementById('createFlowTaskModal');
        if (!modalElement) {
            console.error('createFlowTaskModal element not found');
            return;
        }

        fetch(`/FlowTaskView/CreateModal?taskId=${context.taskId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                const modalBody = modalElement.querySelector('.modal-body');
                if (modalBody) {
                    modalBody.innerHTML = html;
                }
                wireCreateForm(modalElement, context);
                const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
                modal.show();
            })
            .catch(error => {
                console.error('Error loading create flow task modal:', error);
                if (window.showNotification) {
                    window.showNotification('Error loading create flow task form.', 'danger');
                }
            });
    }

    function openEditModal(options) {
        const context = resolveContext(options);
        if (!context || !context.taskId || !context.flowTaskId) {
            if (window.showNotification) {
                window.showNotification('Cannot open flow task modal due to missing information.', 'danger');
            }
            return;
        }

        const modalElement = document.getElementById('editFlowTaskModal');
        if (!modalElement) {
            console.error('editFlowTaskModal element not found');
            return;
        }

        fetch(`/FlowTaskView/EditModal?taskId=${context.taskId}&flowTaskId=${context.flowTaskId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                const modalBody = modalElement.querySelector('.modal-body');
                if (modalBody) {
                    modalBody.innerHTML = html;
                }

                const cache = flowTaskCache.get(context.taskId) || [];
                const flowTask = cache.find(item => item.id === context.flowTaskId);

                wireEditForm(modalElement, context, flowTask);

                const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
                modal.show();
            })
            .catch(error => {
                console.error('Error loading edit flow task modal:', error);
                if (window.showNotification) {
                    window.showNotification('Error loading edit flow task form.', 'danger');
                }
            });
    }

    function deleteFlowTask(options) {
        const context = resolveContext(options);
        if (!context || !context.taskId || !context.flowTaskId) {
            return;
        }

        if (!confirm('Are you sure you want to delete this flow task?')) {
            return;
        }

        fetch(`/api/tasks/${context.taskId}/flow-tasks/${context.flowTaskId}`, {
            method: 'DELETE'
        })
        .then(response => {
            if (response.ok) {
                if (window.showNotification) {
                    window.showNotification('Flow task deleted successfully.', 'success');
                }
                refreshFlowTasks(context);
            } else if (response.status === 404) {
                throw new Error('Flow task not found');
            } else {
                throw new Error('Failed to delete flow task');
            }
        })
        .catch(error => {
            console.error('Error deleting flow task:', error);
            if (window.showNotification) {
                window.showNotification(`Error deleting flow task: ${error.message}`, 'danger');
            }
        });
    }

    document.addEventListener('DOMContentLoaded', () => {
        ['createFlowTaskModal', 'editFlowTaskModal'].forEach(modalId => {
            const modalElement = document.getElementById(modalId);
            if (modalElement) {
                modalElement.addEventListener('hidden.bs.modal', () => {
                    if (typeof cleanupModal === 'function') {
                        cleanupModal(modalId);
                    }
                });
            }
        });
    });

    window.flowTaskManager = {
        refreshFlowTasks,
        openCreateModal,
        openEditModal,
        deleteFlowTask
    };
})();
