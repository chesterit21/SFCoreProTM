
import { truncateText } from '../utils/domUtils.js';

export function createModuleCard(module) {
    return `
        <div class="col-lg-12 mb-2">
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title">${module.name || 'No name'}</h5>
                    <p class="card-text">${truncateText(module.description || 'No description', 100)}</p>
                    <p class="card-text"><small class="text-muted">Sort Order: ${module.sortOrder || 0}</small></p>
                    <div class="d-flex justify-content-end">
                        <div class="btn-group">
                            <button class="btn btn-outline-dark-sm view-module-btn" data-module-id="${module.id}">View</button>
                            <button class="btn btn-outline-primary-sm edit-module-btn" data-module-id="${module.id}">Edit</button>
                            <button class="btn btn-outline-danger-sm delete-module-btn" data-module-id="${module.id}">Delete</button>
                            <button class="btn btn-outline-secondary-sm list-tasks-btn" data-module-id="${module.id}">List Of Tasks</button>
                        </div>
                    </div>
                    <hr />
                    <div class="tasks-container mt-3" id="tasks-${module.id}" style="display: none;">
                        <!-- Tasks will be loaded here -->
                    </div>
                </div>
            </div>
        </div>
    `;
}

export function renderModules(modulesListElement, modules) {
    if (!modulesListElement) return;

    if (modules && modules.length > 0) {
        const modulesHtml = modules.map(createModuleCard).join('');
        modulesListElement.innerHTML = `<div class="row">${modulesHtml}</div>`;
    } else {
        modulesListElement.innerHTML = '<p>No modules found.</p>';
    }
}
