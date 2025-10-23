
import * as flowTaskApi from '../services/flowTaskApiService.js';
import * as flowTaskTpl from '../templates/flowTaskTemplates.js';
import { showNotification } from '../utils/notifications.js';
import { openModal } from './modalHandler.js';

const flowTaskCache = new Map();

async function handleLoadFlowTasks(context) {
    const container = document.getElementById(context.containerId);
    if (!container) return;

    const flowTasks = await flowTaskApi.getFlowTasks(context.taskId);
    flowTaskCache.set(context.taskId, flowTasks); // Cache the results
    flowTaskTpl.renderFlowTaskTable(container, flowTasks, context);
}

async function handleDeleteFlowTask(context) {
    if (!confirm('Are you sure you want to delete this flow task?')) return;

    try {
        await flowTaskApi.deleteFlowTask(context.taskId, context.flowTaskId);
        showNotification('Flow task deleted successfully.', 'success');
        handleLoadFlowTasks(context);
    } catch (error) {
        showNotification(`Error deleting flow task: ${error.message}`, 'danger');
    }
}

function handleCreateFlowTask(context) {
    const { taskId } = context;
    const url = `/FlowTaskView/CreateModal?taskId=${taskId}`;

    openModal('createFlowTaskModal', url, (modalElement) => {
        const form = modalElement.querySelector('#createFlowTaskForm');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(form);
            const requestData = {
                TaskId: taskId,
                Name: formData.get('name'),
                Description: formData.get('description'),
                SortOrder: parseInt(formData.get('sortOrder'), 10) || 1,
                WorkspaceId: context.workspaceId,
                ProjectId: context.projectId,
                ModuleId: context.moduleId
            };

            try {
                await flowTaskApi.createFlowTask(taskId, requestData);
                bootstrap.Modal.getInstance(modalElement).hide();
                showNotification('Flow task created successfully.', 'success');
                handleLoadFlowTasks(context);
            } catch (error) {
                showNotification(`Error creating flow task: ${error.message}`, 'danger');
            }
        });
    });
}

function handleEditFlowTask(context) {
    const { taskId, flowTaskId } = context;
    const url = `/FlowTaskView/EditModal?taskId=${taskId}&flowTaskId=${flowTaskId}`;

    openModal('editFlowTaskModal', url, (modalElement) => {
        const form = modalElement.querySelector('#editFlowTaskForm');
        if (!form) return;

        // Populate form with cached data
        const cachedFlowTasks = flowTaskCache.get(taskId) || [];
        const flowTaskData = cachedFlowTasks.find(ft => ft.id === flowTaskId);
        if (flowTaskData) {
            form.querySelector('[name="name"]').value = flowTaskData.name || '';
            form.querySelector('[name="description"]').value = flowTaskData.description || '';
            form.querySelector('[name="sortOrder"]').value = flowTaskData.sortOrder || 1;
        }

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(form);
            const requestData = {
                Name: formData.get('name'),
                Description: formData.get('description'),
                SortOrder: parseInt(formData.get('sortOrder'), 10) || 1,
                // Include context IDs for the API
                WorkspaceId: context.workspaceId,
                ProjectId: context.projectId,
                ModuleId: context.moduleId,
                TaskId: taskId
            };

            try {
                await flowTaskApi.updateFlowTask(taskId, flowTaskId, requestData);
                bootstrap.Modal.getInstance(modalElement).hide();
                showNotification('Flow task updated successfully.', 'success');
                handleLoadFlowTasks(context);
            } catch (error) {
                showNotification(`Error updating flow task: ${error.message}`, 'danger');
            }
        });
    });
}

function attachFlowTaskActionListeners(container, context) {
    container.addEventListener('click', (e) => {
        const target = e.target;
        const buttonContext = {
            ...context,
            flowTaskId: target.closest('[data-flow-task-id]')?.dataset.flowTaskId
        };

        if (target.classList.contains('edit-flow-task-btn')) {
            handleEditFlowTask(buttonContext);
        } else if (target.classList.contains('delete-flow-task-btn')) {
            handleDeleteFlowTask(buttonContext);
        }
    });
}

export function initFlowTaskSection(context) {
    const container = document.getElementById(context.containerId);
    if (!container) return;

    handleLoadFlowTasks(context);
    attachFlowTaskActionListeners(container, context);

    // This button is on the parent accordion, not inside the flow task list container
    const addFlowTaskButton = document.querySelector(`.accordion-action-btn.add-flow-task-btn[data-task-id="${context.taskId}"]`);
    if (addFlowTaskButton) {
        // To prevent multiple listeners, we clone and replace the button
        const newButton = addFlowTaskButton.cloneNode(true);
        addFlowTaskButton.parentNode.replaceChild(newButton, addFlowTaskButton);
        newButton.addEventListener('click', () => handleCreateFlowTask(context));
    }
}
