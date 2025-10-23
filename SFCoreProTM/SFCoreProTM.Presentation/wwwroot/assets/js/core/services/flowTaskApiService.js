
import { showNotification } from '../utils/notifications.js';

const handleResponse = async (response) => {
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
    }
    return response.json();
};

export const getFlowTasks = async (taskId) => {
    try {
        const response = await fetch(`/api/tasks/${taskId}/flow-tasks`);
        return await handleResponse(response);
    } catch (error) {
        console.error('Error loading flow tasks:', error);
        showNotification('Error loading flow tasks.', 'danger');
        return [];
    }
};

export const createFlowTask = async (taskId, flowTaskData) => {
    const response = await fetch(`/api/tasks/${taskId}/flow-tasks`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(flowTaskData)
    });
    return await handleResponse(response);
};

export const updateFlowTask = async (taskId, flowTaskId, flowTaskData) => {
    const response = await fetch(`/api/tasks/${taskId}/flow-tasks/${flowTaskId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(flowTaskData)
    });
    return await handleResponse(response);
};

export const deleteFlowTask = async (taskId, flowTaskId) => {
    const response = await fetch(`/api/tasks/${taskId}/flow-tasks/${flowTaskId}`, {
        method: 'DELETE'
    });
    if (!response.ok) {
        throw new Error('Failed to delete flow task');
    }
    return response;
};
