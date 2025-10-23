
import { showNotification } from '../utils/notifications.js';

const handleResponse = async (response) => {
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
    }
    return response.json();
};

export const getModules = async (projectId) => {
    try {
        const response = await fetch(`/api/projects/${projectId}/modules`);
        return await handleResponse(response);
    } catch (error) {
        console.error('Error loading modules:', error);
        showNotification('Error loading modules. Please try again later.', 'danger');
        return [];
    }
};

export const createModule = async (projectId, moduleData) => {
    const response = await fetch(`/api/projects/${projectId}/modules`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(moduleData)
    });
    return await handleResponse(response);
};

export const getModuleDetails = async (projectId, moduleId) => {
    try {
        const response = await fetch(`/api/projects/${projectId}/modules`);
        const modules = await handleResponse(response);
        return modules.find(m => m.id === moduleId);
    } catch (error) {
        console.error('Error loading module details:', error);
        showNotification('Error loading module details.', 'danger');
        return null;
    }
};

export const updateModule = async (projectId, moduleId, moduleData) => {
    const response = await fetch(`/api/projects/${projectId}/modules/${moduleId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(moduleData)
    });
    return await handleResponse(response);
};

export const deleteModule = async (projectId, moduleId) => {
    const response = await fetch(`/api/projects/${projectId}/modules/${moduleId}`, {
        method: 'DELETE'
    });
    if (!response.ok) {
        throw new Error('Failed to delete module');
    }
    // DELETE might not return a body, so we don't use handleResponse which expects JSON
    return response;
};

export const getTasksForModule = async (moduleId) => {
    const response = await fetch(`/api/modules/${moduleId}/tasks`);
    return await handleResponse(response);
};
