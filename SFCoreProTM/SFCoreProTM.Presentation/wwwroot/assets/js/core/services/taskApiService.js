
import { showNotification } from '../utils/notifications.js';

const handleResponse = async (response) => {
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
    }
    return response.json();
};

export const getTasks = async (moduleId) => {
    try {
        const response = await fetch(`/api/modules/${moduleId}/tasks`);
        return await handleResponse(response);
    } catch (error) {
        console.error('Error loading tasks:', error);
        showNotification('Error loading tasks.', 'danger');
        return [];
    }
};

export const createTask = async (moduleId, taskData) => {
    const response = await fetch(`/api/modules/${moduleId}/tasks`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(taskData)
    });
    return await handleResponse(response);
};

export const updateTask = async (moduleId, taskId, taskData) => {
    const response = await fetch(`/api/modules/${moduleId}/tasks/${taskId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(taskData)
    });
    return await handleResponse(response);
};

export const deleteTask = async (moduleId, taskId) => {
    const response = await fetch(`/api/modules/${moduleId}/tasks/${taskId}`, {
        method: 'DELETE'
    });
    if (!response.ok) {
        throw new Error('Failed to delete task');
    }
    return response;
};

export const getTaskDetails = async (moduleId, taskId) => {
    try {
        const tasks = await getTasks(moduleId);
        return tasks.find(t => t.id === taskId);
    } catch (error) {
        console.error('Error loading task details:', error);
        showNotification('Error loading task details.', 'danger');
        return null;
    }
};
