
import * as taskApi from '../services/taskApiService.js';
import * as taskTpl from '../templates/taskTemplates.js';
import { showNotification } from '../utils/notifications.js';
import { initFlowTaskSection } from './flowTaskEventHandlers.js';
import { initErdSection } from './erdEventHandlers.js';
import { openModal } from './modalHandler.js';

// --- Task Detail Logic ---

async function handleViewTask(context) {
    const { taskId, moduleId } = context;
    const task = await taskApi.getTaskDetails(moduleId, taskId);
    if (!task) {
        showNotification('Task not found.', 'warning');
        return;
    }

    // Render the main detail layout
    taskTpl.renderTaskDetail(task, context);

    // Prepare context for sub-handlers
    const flowTaskContext = {
        ...context,
        containerId: `flow-task-detail-${taskId}`
    };
    const erdContext = {
        ...context,
        containerId: `erd-definition-form-wrapper-${moduleId}-${taskId}`,
        isErdTask: task.isErd
    };

    // Initialize sub-sections
    initFlowTaskSection(flowTaskContext);
    initErdSection(erdContext);
}

// --- Task List Logic ---

async function handleDeleteTask(context) {
    if (!confirm('Are you sure you want to delete this task?')) return;

    try {
        await taskApi.deleteTask(context.moduleId, context.taskId);
        showNotification('Task deleted successfully.', 'success');
        handleLoadTasks(context.moduleId);
        taskTpl.renderTaskDetailPlaceholder();
    } catch (error) {
        showNotification(`Error deleting task: ${error.message}`, 'danger');
    }
}

function handleCreateTask(moduleId) {
    const url = `/TaskView/CreateModal?moduleId=${moduleId}`;
    openModal('createTaskModal', url, (modalElement) => {
        const form = modalElement.querySelector('#createTaskForm');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(form);
            const requestData = {
                ModuleId: moduleId,
                Name: formData.get('name'),
                Description: formData.get('description'),
                IsErd: formData.get('isErd') === 'on',
                SortOrder: parseInt(formData.get('sortOrder'), 10) || 1
            };

            try {
                await taskApi.createTask(moduleId, requestData);
                bootstrap.Modal.getInstance(modalElement).hide();
                showNotification('Task created successfully.', 'success');
                handleLoadTasks(moduleId);
            } catch (error) {
                showNotification(`Error creating task: ${error.message}`, 'danger');
            }
        });
    });
}

function handleEditTask(context) {
    const { taskId, moduleId } = context;
    const url = `/TaskView/EditModal?moduleId=${moduleId}&taskId=${taskId}`;

    openModal('editTaskModal', url, async (modalElement) => {
        const form = modalElement.querySelector('#editTaskForm');
        if (!form) return;

        // Fetch existing data and populate the form
        const taskData = await taskApi.getTaskDetails(moduleId, taskId);
        if (taskData) {
            form.querySelector('#editName').value = taskData.name || '';
            form.querySelector('#editDescription').value = taskData.description || '';
            form.querySelector('#editSortOrder').value = taskData.sortOrder || 1;
            form.querySelector('#editIsErd').checked = taskData.isErd || false;
        }

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(form);
            const requestData = {
                Name: formData.get('name'),
                Description: formData.get('description'),
                IsErd: form.querySelector('#editIsErd').checked,
                SortOrder: parseInt(formData.get('sortOrder'), 10) || 1
            };

            try {
                await taskApi.updateTask(moduleId, taskId, requestData);
                bootstrap.Modal.getInstance(modalElement).hide();
                showNotification('Task updated successfully.', 'success');
                handleLoadTasks(moduleId);
            } catch (error) {
                showNotification(`Error updating task: ${error.message}`, 'danger');
            }
        });
    });
}

function attachTaskActionListeners(tasksContainer, moduleId) {
    tasksContainer.addEventListener('click', (e) => {
        const target = e.target;

        const context = {
            moduleId,
            projectId: tasksContainer.closest('[data-project-id]')?.dataset.projectId,
            workspaceId: tasksContainer.closest('[data-workspace-id]')?.dataset.workspaceId
        };

        const createBtn = target.closest('.create-task-btn');
        if (createBtn) {
            handleCreateTask(moduleId);
            return;
        }

        const editBtn = target.closest('.edit-task-btn');
        if (editBtn) {
            context.taskId = editBtn.dataset.taskId;
            handleEditTask(context);
            return;
        }

        const viewBtn = target.closest('.view-task-btn');
        if (viewBtn) {
            context.taskId = viewBtn.dataset.taskId;
            handleViewTask(context);
            return;
        }

        const deleteBtn = target.closest('.delete-task-btn');
        if (deleteBtn) {
            context.taskId = deleteBtn.dataset.taskId;
            handleDeleteTask(context);
            return;
        }
    });
}

async function handleLoadTasks(moduleId) {
    const tasksContainer = document.getElementById(`tasks-${moduleId}`);
    if (!tasksContainer) return;

    const tasks = await taskApi.getTasks(moduleId);
    taskTpl.renderTaskTable(tasksContainer, tasks, moduleId);
    attachTaskActionListeners(tasksContainer, moduleId);
}

// --- Public API for this module ---

export function toggleTasksList(moduleId) {
    const tasksContainer = document.getElementById(`tasks-${moduleId}`);
    if (!tasksContainer) return;

    const isVisible = tasksContainer.style.display === 'block';
    if (!isVisible) {
        tasksContainer.style.display = 'block';
        handleLoadTasks(moduleId);
    } else {
        tasksContainer.style.display = 'none';
    }
}
