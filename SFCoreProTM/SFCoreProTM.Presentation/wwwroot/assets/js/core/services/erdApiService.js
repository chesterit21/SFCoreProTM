
import { showNotification } from '../utils/notifications.js';

const handleResponse = async (response) => {
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
    }
    return response.json();
};

const handleEmptyResponse = (response) => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    return response;
};

export const getErdDefinitions = async (moduleId) => {
    try {
        const response = await fetch(`/api/modules/${moduleId}/erd-definitions`);
        return await handleResponse(response);
    } catch (error) {
        console.error('Error loading ERD definitions:', error);
        showNotification('Error loading ERD definitions.', 'danger');
        return [];
    }
};

export const createErdDefinition = async (moduleId, data) => {
    const response = await fetch(`/api/modules/${moduleId}/erd-definitions`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    return await handleResponse(response);
};

export const updateErdDefinition = async (moduleId, erdId, data) => {
    const response = await fetch(`/api/modules/${moduleId}/erd-definitions/${erdId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    return await handleResponse(response);
};

export const deleteErdDefinition = async (moduleId, erdId) => {
    const response = await fetch(`/api/modules/${moduleId}/erd-definitions/${erdId}`, { 
        method: 'DELETE' 
    });
    return handleEmptyResponse(response);
};
