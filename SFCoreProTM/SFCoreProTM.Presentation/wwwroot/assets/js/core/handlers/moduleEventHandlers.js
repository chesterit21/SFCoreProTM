
import {
    getModules,
    createModule,
    getModuleDetails,
    updateModule,
    deleteModule,
    getTasksForModule
} from '../services/moduleApiService.js';
import { renderModules } from '../templates/moduleTemplates.js';
import { showNotification } from '../utils/notifications.js';
import { cleanupModal } from '../utils/domUtils.js';
import { toggleTasksList } from './taskEventHandlers.js';

async function handleLoadModules(modulesListElement) {
    const projectId = modulesListElement.getAttribute('data-project-id');
    if (!projectId || projectId === 'null') {
        console.error('Project ID not found');
        modulesListElement.innerHTML = '<p>Error: Project ID not found.</p>';
        return;
    }

    const modules = await getModules(projectId);
    renderModules(modulesListElement, modules);
    attachModuleActionListeners(modulesListElement);
}

async function handleViewModule(moduleId) {
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');
    const projectName = modulesList?.getAttribute('data-project-name');

    const modalElement = document.getElementById('viewModuleModal');
    if (!modalElement) return;

    try {
        const moduleData = await getModuleDetails(projectId, moduleId);
        if (moduleData) {
            modalElement.querySelector('#viewProjectName').textContent = projectName || '';
            modalElement.querySelector('#viewModuleName').textContent = moduleData.name || '';
            modalElement.querySelector('#viewModuleDescription').innerHTML = moduleData.description || ''; // Use innerHTML for rich text
            modalElement.querySelector('#viewModuleSortOrder').textContent = moduleData.sortOrder || 'N/A';

            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        } else {
            showNotification('Could not find module details.', 'warning');
        }
    } catch (error) {
        showNotification(`Error fetching module details: ${error.message}`, 'danger');
    }
}

async function handleEditModule(moduleId) {
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');

    try {
        const response = await fetch(`/ModuleView/EditModal?projectId=${projectId}&moduleId=${moduleId}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        const html = await response.text();

        const modalElement = document.getElementById('editModuleModal');
        const modalBody = modalElement.querySelector('.modal-body');
        modalBody.innerHTML = html;

        const moduleData = await getModuleDetails(projectId, moduleId);
        if (moduleData) {
            modalElement.querySelector('#name').value = moduleData.name || '';
            modalElement.querySelector('#description').value = moduleData.description || '';
            modalElement.querySelector('#sortOrder').value = moduleData.sortOrder || 1;
        }

        const form = modalElement.querySelector('#editModuleForm');
        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(form);
            const requestData = {
                name: formData.get('name'),
                description: formData.get('description'),
                sortOrder: parseInt(formData.get('sortOrder'), 10) || 1
            };

            try {
                await updateModule(projectId, moduleId, requestData);
                bootstrap.Modal.getInstance(modalElement).hide();
                showNotification('Module updated successfully.', 'success');
                handleLoadModules(modulesList);
            } catch (error) {
                showNotification(`Error updating module: ${error.message}`, 'danger');
            }
        });

        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } catch (error) {
        showNotification(`Error loading edit form: ${error.message}`, 'danger');
    }
}

async function handleDeleteModule(moduleId) {
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');

    try {
        const tasks = await getTasksForModule(moduleId);
        const tasksInProgress = tasks.some(task => task.status === 1); // Assuming 1 is InProgress

        if (tasksInProgress) {
            showNotification('Cannot delete module. There are tasks in progress.', 'warning');
            return;
        }

        if (confirm('Are you sure you want to delete this module?')) {
            await deleteModule(projectId, moduleId);
            showNotification('Module deleted successfully.', 'success');
            handleLoadModules(modulesList);
        }
    } catch (error) {
        showNotification(`Error deleting module: ${error.message}`, 'danger');
    }
}

function attachModuleActionListeners(modulesListElement) {
    modulesListElement.addEventListener('click', (event) => {
        const target = event.target;
        const moduleId = target.getAttribute('data-module-id');

        if (target.classList.contains('view-module-btn')) {
            handleViewModule(moduleId);
        } else if (target.classList.contains('edit-module-btn')) {
            handleEditModule(moduleId);
        } else if (target.classList.contains('delete-module-btn')) {
            handleDeleteModule(moduleId);
        } else if (target.classList.contains('list-tasks-btn')) {
            toggleTasksList(moduleId);
        }
    });
}

export function initializeCreateModuleButton() {
    const createModuleBtn = document.getElementById('createModuleBtn');
    if (!createModuleBtn) return;

    createModuleBtn.addEventListener('click', async () => {
        const modulesList = document.getElementById('modulesList');
        const projectId = modulesList?.getAttribute('data-project-id');

        try {
            const response = await fetch(`/ModuleView/CreateModal?projectId=${projectId}`);
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            const html = await response.text();

            const modalElement = document.getElementById('createModuleModal');
            const modalBody = modalElement.querySelector('.modal-body');
            modalBody.innerHTML = html;

            const form = modalElement.querySelector('#createModuleForm');
            form.addEventListener('submit', async (e) => {
                e.preventDefault();
                const formData = new FormData(form);
                const requestData = {
                    projectId: projectId,
                    name: formData.get('name'),
                    description: formData.get('description'),
                    sortOrder: parseInt(formData.get('sortOrder'), 10) || 1
                };

                try {
                    await createModule(projectId, requestData);
                    bootstrap.Modal.getInstance(modalElement).hide();
                    showNotification('Module created successfully.', 'success');
                    handleLoadModules(modulesList);
                } catch (error) {
                    showNotification(`Error creating module: ${error.message}`, 'danger');
                }
            });

            const modal = new bootstrap.Modal(modalElement);
            modal.show();

        } catch (error) {
            showNotification(`Error loading create form: ${error.message}`, 'danger');
        }
    });
}

export function initializeModals() {
    ['createModuleModal', 'editModuleModal'].forEach(modalId => {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.addEventListener('hidden.bs.modal', () => cleanupModal(modalId));
        }
    });
}

export function initPage() {
    const modulesListElement = document.getElementById('modulesList');
    if (modulesListElement) {
        handleLoadModules(modulesListElement);
    }
    initializeCreateModuleButton();
    initializeModals();
}
