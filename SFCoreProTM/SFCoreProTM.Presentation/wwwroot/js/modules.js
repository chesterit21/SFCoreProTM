if (typeof window.showNotification !== 'function') {
    window.showNotification = function (message, type = 'info') {
        const allowedTypes = new Set(['primary', 'secondary', 'success', 'danger', 'warning', 'info', 'light', 'dark']);
        const resolvedType = allowedTypes.has(type) ? type : 'info';

        let container = document.getElementById('notificationContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'notificationContainer';
            container.className = 'notification-container';
            document.body.appendChild(container);
        }

        const alertElement = document.createElement('div');
        alertElement.className = `alert alert-${resolvedType} alert-dismissible fade show`;
        alertElement.setAttribute('role', 'alert');
        alertElement.innerHTML = `
            <div>${message}</div>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

        container.appendChild(alertElement);

        const autoDismiss = setTimeout(() => {
            try {
                const instance = bootstrap.Alert.getOrCreateInstance(alertElement);
                instance.close();
            } catch (err) {
                alertElement.remove();
            }
        }, 4000);

        alertElement.addEventListener('close.bs.alert', () => clearTimeout(autoDismiss));
        alertElement.addEventListener('closed.bs.alert', () => alertElement.remove());
    };
}

document.addEventListener('DOMContentLoaded', function () {
    // Load modules when page loads
    loadModules();

    // Tambahkan event listener untuk membersihkan modal saat ditutup
    const createModal = document.getElementById('createModuleModal');
    if (createModal) {
        createModal.addEventListener('hidden.bs.modal', function () {
            cleanupModal('createModuleModal');
        });
    }

    const editModal = document.getElementById('editModuleModal');
    if (editModal) {
        editModal.addEventListener('hidden.bs.modal', function () {
            cleanupModal('editModuleModal');
        });
    }

    // Load create modal content when create button is clicked
    const createModuleBtn = document.getElementById('createModuleBtn');
    if (createModuleBtn) {
        createModuleBtn.addEventListener('click', function () {
            loadCreateModuleModal();
        });
    }
});

// Fungsi untuk memotong teks
function truncateText(text, maxLength) {
    if (text.length <= maxLength) {
        return text;
    }
    return text.substring(0, maxLength) + '...';
}

// Fungsi untuk toggle tampilan daftar task
function toggleTasksList(moduleId) {
    const tasksContainer = document.getElementById(`tasks-${moduleId}`);
    if (tasksContainer) {
        if (tasksContainer.style.display === 'none' || tasksContainer.style.display === '') {
            // Expand tasks list
            tasksContainer.style.display = 'block';
            // Memanggil fungsi loadTasks dari file task.js
            if (typeof loadTasks === 'function') {
                loadTasks(moduleId);
            } else {
                console.error('loadTasks function is not accessible');
            }
        } else {
            // Collapse tasks list
            tasksContainer.style.display = 'none';
        }
    }
}

// Fungsi untuk memuat modal pembuatan module
function loadCreateModuleModal() {
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');
    const workspaceId = modulesList?.getAttribute('data-workspace-id');
    
    if (!projectId || projectId === 'null') {
        console.error('Project ID not found');
        showNotification('Project ID not found.', 'danger');
        return;
    }
    
    fetch(`/ModuleView/CreateModal?projectId=${projectId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            const modalElement = document.getElementById('createModuleModal');
            if (modalElement) {
                const modalBody = modalElement.querySelector('.modal-body');
                if (modalBody) {
                    modalBody.innerHTML = html;
                }

                const form = modalElement.querySelector('#createModuleForm');
                if (form) {
                    const projectField = form.querySelector('input[name="projectId"]');
                    if (projectField) {
                        projectField.value = projectId;
                    }
                    const workspaceField = form.querySelector('input[name="workspaceId"]');
                    if (workspaceField && workspaceId && workspaceId !== 'null') {
                        workspaceField.value = workspaceId;
                    }

                    form.addEventListener('submit', function (e) {
                        e.preventDefault();
                        
                        const formData = new FormData(form);
                        const requestData = {
                            projectId: projectId,
                            name: formData.get('name'),
                            description: formData.get('description'),
                            sortOrder: parseInt(formData.get('sortOrder'), 10) || 1
                        };
                        
                        fetch(`/api/projects/${projectId}/modules`, {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify(requestData)
                        })
                        .then(response => {
                            if (!response.ok) {
                                return response.text().then(text => {
                                    throw new Error(`HTTP error! status: ${response.status}, message: ${text}`);
                                });
                            }
                            return response.json();
                        })
                        .then(data => {
                            const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
                            modalInstance.hide();

                            if (typeof loadModules === 'function') {
                                loadModules();
                            } else {
                                console.error('loadModules function is not accessible');
                            }

                            showNotification('Module created successfully.', 'success');
                        })
                        .catch(error => {
                            showNotification(`Error creating module: ${error.message}`, 'danger');
                            console.error('Error:', error);
                        });
                    });
                }

                const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
                modal.show();
            } else {
                console.error('Create module modal element not found');
                showNotification('Create module modal is not available.', 'danger');
            }
        })
        .catch(error => {
            console.error('Error loading create modal:', error);
            showNotification('Error loading create module form.', 'danger');
        });
}

// Fungsi untuk melihat detail module
function viewModuleDetails(moduleId) {
    // Implementation for viewing module details
    console.log('View module details for ID:', moduleId);
}

// Fungsi untuk memuat modal edit module
function loadEditModuleModal(moduleId) {
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');
    const workspaceId = modulesList?.getAttribute('data-workspace-id');

    if (!projectId || projectId === 'null') {
        console.error('Project ID not found');
        showNotification('Project ID not found.', 'danger');
        return;
    }

    fetch(`/ModuleView/EditModal?projectId=${projectId}&moduleId=${moduleId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            const modalElement = document.getElementById('editModuleModal');
            if (!modalElement) {
                console.error('Edit module modal element not found');
                showNotification('Edit module modal is not available.', 'danger');
                return;
            }

            const modalBody = modalElement.querySelector('.modal-body');
            if (modalBody) {
                modalBody.innerHTML = html;
            }

            const form = modalElement.querySelector('#editModuleForm');
            if (form) {
                const projectField = form.querySelector('input[name="projectId"]');
                if (projectField) {
                    projectField.value = projectId;
                }
                const moduleField = form.querySelector('input[name="moduleId"]');
                if (moduleField) {
                    moduleField.value = moduleId;
                }
                const workspaceField = form.querySelector('input[name="workspaceId"]');
                if (workspaceField && workspaceId && workspaceId !== 'null') {
                    workspaceField.value = workspaceId;
                }

                fetch(`/api/projects/${projectId}/modules`)
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(`HTTP error! status: ${response.status}`);
                        }
                        return response.json();
                    })
                    .then(modules => {
                        const targetModule = modules.find(m => m.id === moduleId);
                        if (targetModule) {
                            const nameInput = form.querySelector('#name');
                            if (nameInput) {
                                nameInput.value = targetModule.name || '';
                            }
                            const descriptionInput = form.querySelector('#description');
                            if (descriptionInput) {
                                descriptionInput.value = targetModule.description || '';
                            }
                            const sortOrderInput = form.querySelector('#sortOrder');
                            if (sortOrderInput) {
                                sortOrderInput.value = targetModule.sortOrder || 1;
                            }
                        } else {
                            showNotification('Module data could not be loaded.', 'warning');
                        }
                    })
                    .catch(error => {
                        console.error('Error loading module data:', error);
                        showNotification(`Error loading module data: ${error.message}`, 'danger');
                    });

                form.addEventListener('submit', function (e) {
                    e.preventDefault();

                    const formData = new FormData(form);
                    const requestData = {
                        name: formData.get('name'),
                        description: formData.get('description'),
                        sortOrder: parseInt(formData.get('sortOrder'), 10) || 1
                    };

                    fetch(`/api/projects/${projectId}/modules/${moduleId}`, {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(requestData)
                    })
                    .then(response => {
                        if (!response.ok) {
                            return response.text().then(text => {
                                throw new Error(`HTTP error! status: ${response.status}, message: ${text}`);
                            });
                        }
                        return response.json();
                    })
                    .then(data => {
                        const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
                        modalInstance.hide();

                        if (typeof loadModules === 'function') {
                            loadModules();
                        }

                        showNotification('Module updated successfully.', 'success');
                    })
                    .catch(error => {
                        console.error('Error updating module:', error);
                        showNotification(`Error updating module: ${error.message}`, 'danger');
                    });
                });
            }

            const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
            modal.show();
        })
        .catch(error => {
            console.error('Error loading edit modal:', error);
            showNotification('Error loading edit module form.', 'danger');
        });
}

// Fungsi untuk menghapus module
function deleteModule(moduleId) {
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');
    
    if (!projectId || projectId === 'null') {
        console.error('Project ID not found');
        showNotification('Project ID not found.', 'danger');
        return;
    }

    // Check if there are tasks in progress before deleting
    fetch(`/api/modules/${moduleId}/tasks`)
        .then(response => response.json())
        .then(tasks => {
            const tasksInProgress = tasks.some(task => task.status === 1); // Assuming 1 is InProgress status
            
            if (tasksInProgress) {
                showNotification('Cannot delete module. There are tasks in progress.', 'warning');
                return;
            }
            
            if (confirm('Are you sure you want to delete this module? All tasks, flow tasks, and ERD definitions will also be deleted.')) {
                fetch(`/api/projects/${projectId}/modules/${moduleId}`, {
                    method: 'DELETE'
                })
                    .then(response => {
                        if (response.ok) {
                            loadModules(); // Reload modules list
                            showNotification('Module deleted successfully.', 'success');
                        } else if (response.status === 404) {
                            throw new Error('Module not found');
                        } else {
                            throw new Error('Failed to delete module');
                        }
                    })
                    .catch(error => {
                        console.error('Error deleting module:', error);
                        showNotification(`Error deleting module: ${error.message}`, 'danger');
                    });
            }
        })
        .catch(error => {
            console.error('Error checking tasks:', error);
            showNotification(`Error checking tasks: ${error.message}`, 'danger');
        });
}

// Define loadModules function in global scope
function loadModules() {
    // Get project ID from the data attribute
    const modulesList = document.getElementById('modulesList');
    const projectId = modulesList?.getAttribute('data-project-id');
    const projectName = modulesList?.getAttribute('data-project-name') || 'Project';
    
    if (!projectId || projectId === 'null') {
        console.error('Project ID not found');
        if (modulesList) {
            modulesList.innerHTML = '<p>Error: Project ID not found.</p>';
        }
        return;
    }

    fetch(`/api/projects/${projectId}/modules`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            let modulesHtml = '';
            if (data && data.length > 0) {
                modulesHtml = '<div class="row">';
                data.forEach(function (module) {
                    modulesHtml += `
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
                });
                modulesHtml += '</div>';
            } else {
                modulesHtml = '<p>No modules found.</p>';
            }

            if (modulesList) {
                modulesList.innerHTML = modulesHtml;

                // Attach event handlers for view module buttons
                modulesList.querySelectorAll('.view-module-btn').forEach(button => {
                    button.addEventListener('click', function () {
                        const moduleId = this.getAttribute('data-module-id');
                        viewModuleDetails(moduleId);
                    });
                });

                // Attach event handlers for edit module buttons
                modulesList.querySelectorAll('.edit-module-btn').forEach(button => {
                    button.addEventListener('click', function () {
                        const moduleId = this.getAttribute('data-module-id');
                        loadEditModuleModal(moduleId);
                    });
                });

                // Attach event handlers for delete module buttons
                modulesList.querySelectorAll('.delete-module-btn').forEach(button => {
                    button.addEventListener('click', function () {
                        const moduleId = this.getAttribute('data-module-id');
                        deleteModule(moduleId);
                    });
                });

                // Attach event handlers for list tasks buttons
                modulesList.querySelectorAll('.list-tasks-btn').forEach(button => {
                    button.addEventListener('click', function () {
                        const moduleId = this.getAttribute('data-module-id');
                        toggleTasksList(moduleId);
                    });
                });
            }

            // Update project title
            const projectTitle = document.getElementById('projectTitle');
            if (projectTitle) {
                projectTitle.textContent = projectName;
            }
        })
        .catch(error => {
            console.error('Error loading modules:', error);
            if (modulesList) {
                modulesList.innerHTML = '<p>Error loading modules. Please try again later.</p>';
            }
        });
}

// Fungsi untuk membersihkan modal setelah ditutup
function cleanupModal(modalId) {
    const modalElement = document.getElementById(modalId);
    if (modalElement) {
        // Hapus fokus dari elemen dalam modal
        const focusedElement = document.activeElement;
        if (focusedElement && modalElement.contains(focusedElement)) {
            focusedElement.blur();
        }

        // Hapus atribut aria-hidden
        modalElement.removeAttribute('aria-hidden');

        // Bersihkan konten modal
        const modalBody = modalElement.querySelector('.modal-body');
        if (modalBody) {
            modalBody.innerHTML = '';
        }
    }
}

// Memuat daftar module saat halaman dimuat
document.addEventListener('DOMContentLoaded', function () {
    loadModules();
});
