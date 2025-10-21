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

// Fungsi untuk memotong teks
function truncateText(text, maxLength) {
    if (text.length <= maxLength) {
        return text;
    }
    return text.substring(0, maxLength) + '...';
}

// Normalisasi flag ERD dari response API (mendukung camelCase dan PascalCase)
function resolveTaskIsErd(task) {
    return Boolean(task?.isErd ?? task?.IsErd);
}

function escapeHtml(value) {
    if (value === null || value === undefined) {
        return '';
    }
    return String(value)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

let attributeRowCounter = 0;

function createAttributeRowElement(context, attribute = {}) {
    const row = document.createElement('div');
    row.className = 'card border-0 shadow-sm attribute-row';
    row.dataset.attributeId = attribute?.id ?? attribute?.Id ?? '';

    const rowIndex = attributeRowCounter++;
    const nameValue = attribute?.name ?? attribute?.Name ?? '';
    const dataTypeValue = attribute?.dataType ?? attribute?.DataType ?? '';
    const descriptionValue = attribute?.description ?? attribute?.Description ?? '';
    const maxCharValue = attribute?.maxChar ?? attribute?.MaxChar;
    const sortOrderValue = attribute?.sortOrder ?? attribute?.SortOrder;
    const isPrimaryValue = Boolean(attribute?.isPrimary ?? attribute?.IsPrimary);
    const isNullValue = Boolean(attribute?.isNull ?? attribute?.IsNull);
    const isForeignKeyValue = Boolean(attribute?.isForeignKey ?? attribute?.IsForeignKey);
    const foreignKeyTableValue = attribute?.foreignKeyTable ?? attribute?.ForeignKeyTable ?? '';

    const formId = context.formId ?? `erd-form-${Date.now()}`;

    row.innerHTML = `
        <div class="card-body">
            <div class="row g-3 align-items-end">
                <div class="col-md-4">
                    <label class="form-label" for="${formId}-attr-${rowIndex}-name">Attribute Name</label>
                    <input type="text" class="form-control" id="${formId}-attr-${rowIndex}-name" data-field="name" value="${escapeHtml(nameValue)}">
                </div>
                <div class="col-md-3">
                    <label class="form-label" for="${formId}-attr-${rowIndex}-datatype">Data Type</label>
                    <input type="text" class="form-control" id="${formId}-attr-${rowIndex}-datatype" data-field="dataType" value="${escapeHtml(dataTypeValue)}">
                </div>
                <div class="col-md-3">
                    <label class="form-label" for="${formId}-attr-${rowIndex}-sortorder">Sort Order</label>
                    <input type="number" class="form-control" id="${formId}-attr-${rowIndex}-sortorder" data-field="sortOrder" value="${sortOrderValue ?? ''}" min="0">
                </div>
                <div class="col-md-2 text-end">
                    <button type="button" class="btn btn-outline-danger-sm" data-action="remove-attribute-row">
                        <i class="fas fa-times me-1"></i>Remove
                    </button>
                </div>
                <div class="col-12">
                    <label class="form-label" for="${formId}-attr-${rowIndex}-description">Attribute Description</label>
                    <textarea class="form-control" id="${formId}-attr-${rowIndex}-description" data-field="description" rows="2">${escapeHtml(descriptionValue)}</textarea>
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="${formId}-attr-${rowIndex}-maxchar">Max Char</label>
                    <input type="number" class="form-control" id="${formId}-attr-${rowIndex}-maxchar" data-field="maxChar" value="${maxCharValue ?? ''}" min="0">
                </div>
                <div class="col-md-8">
                    <div class="row g-3">
                        <div class="col-md-4">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="${formId}-attr-${rowIndex}-isprimary" data-field="isPrimary" ${isPrimaryValue ? 'checked' : ''}>
                                <label class="form-check-label" for="${formId}-attr-${rowIndex}-isprimary">Primary Key</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="${formId}-attr-${rowIndex}-isnull" data-field="isNull" ${isNullValue ? 'checked' : ''}>
                                <label class="form-check-label" for="${formId}-attr-${rowIndex}-isnull">Allow Null</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="${formId}-attr-${rowIndex}-isforeign" data-field="isForeignKey" ${isForeignKeyValue ? 'checked' : ''}>
                                <label class="form-check-label" for="${formId}-attr-${rowIndex}-isforeign">Foreign Key</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="${formId}-attr-${rowIndex}-fktable">Foreign Key Table</label>
                    <input type="text" class="form-control" id="${formId}-attr-${rowIndex}-fktable" data-field="foreignKeyTable" value="${escapeHtml(foreignKeyTableValue)}">
                </div>
            </div>
        </div>
    `;

    const removeButton = row.querySelector('[data-action="remove-attribute-row"]');
    removeButton.addEventListener('click', () => {
        if (!context.attributeRowsContainer) {
            return;
        }
        const rows = context.attributeRowsContainer.querySelectorAll('.attribute-row');
        if (rows.length <= 1) {
            row.querySelectorAll('input[data-field], textarea[data-field]').forEach(input => {
                if (input.type === 'checkbox') {
                    input.checked = false;
                } else {
                    input.value = '';
                }
            });
            row.dataset.attributeId = '';
        } else {
            row.remove();
        }
        updateAttributeRowState(context);
    });

    const foreignKeyCheckbox = row.querySelector('[data-field="isForeignKey"]');
    foreignKeyCheckbox.addEventListener('change', () => {
        updateAttributeRowState(context);
    });

    return row;
}

function updateAttributeRowState(context) {
    if (!context.attributeRowsContainer) {
        return;
    }

    const rows = Array.from(context.attributeRowsContainer.querySelectorAll('.attribute-row'));
    const isEditable = Boolean(context.isErdTask);

    rows.forEach(row => {
        const removeButton = row.querySelector('[data-action="remove-attribute-row"]');
        if (removeButton) {
            removeButton.classList.toggle('d-none', !isEditable);
            removeButton.disabled = !isEditable;
        }

        row.querySelectorAll('input[data-field], textarea[data-field]').forEach(input => {
            if (!isEditable) {
                if (input.type === 'checkbox') {
                    input.disabled = true;
                } else {
                    input.readOnly = true;
                }
            } else {
                if (input.type === 'checkbox') {
                    input.disabled = false;
                } else {
                    input.readOnly = false;
                }
            }
        });

        const foreignKeyCheckbox = row.querySelector('[data-field="isForeignKey"]');
        const foreignKeyInput = row.querySelector('[data-field="foreignKeyTable"]');
        if (foreignKeyInput) {
            const shouldDisable = !isEditable || !(foreignKeyCheckbox?.checked ?? false);
            foreignKeyInput.disabled = shouldDisable;
        }
    });

    if (context.addAttributeButton) {
        context.addAttributeButton.disabled = !isEditable;
    }
}

function addAttributeRow(context, attribute) {
    if (!context.attributeRowsContainer) {
        return;
    }

    const row = createAttributeRowElement(context, attribute);
    context.attributeRowsContainer.appendChild(row);
    updateAttributeRowState(context);
}

function populateAttributeRows(context, attributes) {
    if (!context.attributeRowsContainer) {
        return;
    }

    context.attributeRowsContainer.innerHTML = '';
    const items = Array.isArray(attributes) && attributes.length > 0 ? attributes : [null];
    items.forEach(attribute => addAttributeRow(context, attribute));
    updateAttributeRowState(context);
}

function collectAttributeRows(context) {
    if (!context.attributeRowsContainer) {
        return [];
    }

    const rows = Array.from(context.attributeRowsContainer.querySelectorAll('.attribute-row'));
    const attributes = [];

    rows.forEach(row => {
        const name = row.querySelector('[data-field="name"]')?.value?.trim() ?? '';
        const dataType = row.querySelector('[data-field="dataType"]')?.value?.trim() ?? '';
        const description = row.querySelector('[data-field="description"]')?.value?.trim() ?? '';
        const maxCharRaw = row.querySelector('[data-field="maxChar"]')?.value?.trim() ?? '';
        const sortOrderRaw = row.querySelector('[data-field="sortOrder"]')?.value?.trim() ?? '';
        const isPrimary = row.querySelector('[data-field="isPrimary"]')?.checked ?? false;
        const isNull = row.querySelector('[data-field="isNull"]')?.checked ?? false;
        const isForeignKey = row.querySelector('[data-field="isForeignKey"]')?.checked ?? false;
        const foreignKeyTable = row.querySelector('[data-field="foreignKeyTable"]')?.value?.trim() ?? '';

        if (!name && !dataType && !description && !maxCharRaw && !sortOrderRaw && !foreignKeyTable) {
            return;
        }

        const maxChar = maxCharRaw === '' ? null : Number.parseInt(maxCharRaw, 10);
        const sortOrder = sortOrderRaw === '' ? null : Number.parseInt(sortOrderRaw, 10);

        attributes.push({
            id: row.dataset.attributeId || null,
            name,
            dataType,
            description,
            maxChar: Number.isNaN(maxChar) ? null : maxChar,
            sortOrder: Number.isNaN(sortOrder) ? null : sortOrder,
            isPrimary,
            isNull,
            isForeignKey,
            foreignKeyTable: isForeignKey ? foreignKeyTable : ''
        });
    });

    return attributes;
}

function ensureTaskDetailPlaceholder() {
    const detailContainer = document.getElementById('detail-of-task');
    if (detailContainer && !detailContainer.innerHTML.trim()) {
        detailContainer.innerHTML = `
            <div class="alert alert-info shadow-sm" role="alert">
                Select a task from the list to view its details.
            </div>
        `;
    }
}

function renderTaskDetail(task, moduleIdFromContext) {
    const detailContainer = document.getElementById('detail-of-task');
    if (!detailContainer) {
        return;
    }

    const taskId = task?.id ?? task?.Id;
    if (!taskId) {
        showNotification('Task information is incomplete.', 'warning');
        return;
    }

    const isErdTask = resolveTaskIsErd(task);
    const moduleId = task?.moduleId ?? task?.ModuleId ?? moduleIdFromContext;
    const projectId = task?.projectId ?? task?.ProjectId ?? '';
    const workspaceId = task?.workspaceId ?? task?.WorkspaceId ?? '';
    const sortOrder = task?.sortOrder ?? task?.SortOrder ?? 1;
    const statusValue = task?.status ?? task?.Status ?? 'N/A';

    const accordionId = `taskDetailAccordion-${taskId}`;
    const flowCollapseId = `task-detail-flow-${taskId}`;
    const erdCollapseId = `task-detail-erd-${taskId}`;
    const flowContainerId = `flow-task-detail-${taskId}`;
    const erdListId = `erd-definition-list-${moduleId}-${taskId}`;
    const erdFormId = `erd-definition-form-${moduleId}-${taskId}`;
    const erdFormWrapperId = `erd-definition-form-wrapper-${moduleId}-${taskId}`;

    const descriptionHtml = task?.description
        ? `<p class="mb-0">${escapeHtml(task.description)}</p>`
        : '<p class="mb-0 text-muted">No description provided.</p>';

    detailContainer.dataset.currentTaskId = taskId;
    if (moduleId) {
        detailContainer.dataset.currentModuleId = moduleId;
    } else {
        delete detailContainer.dataset.currentModuleId;
    }

    detailContainer.innerHTML = `
        <div class="card shadow-sm mb-3">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start gap-3">
                    <div>
                        <h5 class="mb-1">${escapeHtml(task?.name ?? 'No name')}</h5>
                        ${descriptionHtml}
                    </div>
                    <span class="badge bg-${isErdTask ? 'success' : 'secondary'}">${isErdTask ? 'ERD Task' : 'Non-ERD Task'}</span>
                </div>
                <div class="row g-3 small text-muted mt-3">
                    <div class="col-md-4"><strong>Sort Order:</strong> ${sortOrder}</div>
                    <div class="col-md-4"><strong>Status:</strong> ${escapeHtml(statusValue)}</div>
                    <div class="col-md-4"><strong>Task ID:</strong> <code>${escapeHtml(taskId)}</code></div>
                </div>
            </div>
        </div>
        <div class="accordion accordion-flush" id="${accordionId}">
            <div class="accordion-item border rounded-3 shadow-sm mb-3">
                <h2 class="accordion-header position-relative" id="heading-flow-${taskId}">
                    <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#${flowCollapseId}" aria-expanded="true" aria-controls="${flowCollapseId}">
                        List of Flow Tasks
                    </button>
                    <button class="btn btn-outline-secondary-sm accordion-action-btn"
                        data-action="detail-add-flow-task"
                        data-task-id="${taskId}"
                        data-module-id="${moduleId ?? ''}"
                        data-project-id="${projectId ?? ''}"
                        data-workspace-id="${workspaceId ?? ''}">
                        <i class="fas fa-plus me-1"></i>Add Flow Task
                    </button>
                </h2>
                <div id="${flowCollapseId}" class="accordion-collapse collapse show" data-bs-parent="#${accordionId}">
                    <div class="accordion-body">
                        <div class="flow-tasks-container"
                             id="${flowContainerId}"
                             data-task-id="${taskId}"
                             data-module-id="${moduleId ?? ''}"
                             data-project-id="${projectId ?? ''}"
                             data-workspace-id="${workspaceId ?? ''}">
                            <p class="text-muted small mb-0">Loading flow tasks...</p>
                        </div>
                    </div>
                </div>
            </div>
            <div class="accordion-item border rounded-3 shadow-sm">
                <h2 class="accordion-header" id="heading-erd-${taskId}">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#${erdCollapseId}" aria-expanded="false" aria-controls="${erdCollapseId}">
                        List of ERD Definitions
                    </button>
                </h2>
                <div id="${erdCollapseId}" class="accordion-collapse collapse" data-bs-parent="#${accordionId}">
                    <div class="accordion-body">
                        ${isErdTask ? '' : `
                            <div class="alert alert-warning mb-3" role="alert">
                                This task is currently marked as Non-ERD. You can still review existing ERD definitions, but editing is disabled until the task is flagged as ERD.
                            </div>
                        `}
                        <div class="border rounded-3 p-3 bg-light-subtle" id="${erdFormWrapperId}">
                            <form id="${erdFormId}" class="row g-3">
                                <div class="col-12">
                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <label class="form-label" for="${erdFormId}-tName">Table Name</label>
                                            <input type="text" class="form-control" id="${erdFormId}-tName" name="tName" required>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label" for="${erdFormId}-entityName">Entity Name</label>
                                            <input type="text" class="form-control" id="${erdFormId}-entityName" name="entityName">
                                        </div>
                                        <div class="col-12">
                                            <label class="form-label" for="${erdFormId}-description">Table Description</label>
                                            <textarea class="form-control" id="${erdFormId}-description" name="description" rows="3" placeholder="Describe this entity"></textarea>
                                        </div>
                                        <div class="col-md-4">
                                            <label class="form-label" for="${erdFormId}-sortOrder">Table Sort Order</label>
                                            <input type="number" class="form-control" id="${erdFormId}-sortOrder" name="sortOrder" value="1" min="1">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-12">
                                    <div class="d-flex justify-content-between align-items-center mb-2">
                                        <h6 class="mb-0">Attributes</h6>
                                        <button type="button" class="btn btn-outline-secondary-sm" data-action="add-attribute-row">
                                            <i class="fas fa-plus me-1"></i>Add Attribute
                                        </button>
                                    </div>
                                    <div class="d-flex flex-column gap-3" data-role="attribute-rows"></div>
                                </div>
                                <div class="col-12 d-flex justify-content-end gap-2">
                                    <button type="button" class="btn btn-outline-secondary-sm d-none" data-action="cancel-erd-edit">
                                        Cancel
                                    </button>
                                    <button type="submit" class="btn btn-outline-success-sm" data-role="erd-submit">
                                        <i class="fas fa-save me-1"></i>Save ERD Definition
                                    </button>
                                </div>
                            </form>
                        </div>
                        <div id="${erdListId}" class="mt-3"></div>
                    </div>
                </div>
            </div>
        </div>
    `;

    const addFlowTaskButton = detailContainer.querySelector('[data-action="detail-add-flow-task"]');
    if (addFlowTaskButton && window.flowTaskManager) {
        addFlowTaskButton.addEventListener('click', () => {
            window.flowTaskManager.openCreateModal({
                taskId,
                moduleId,
                projectId,
                workspaceId
            });
        });
    }

    const flowTasksContainer = document.getElementById(flowContainerId);
    if (flowTasksContainer && window.flowTaskManager) {
        window.flowTaskManager.refreshFlowTasks({
            containerId: flowContainerId,
            taskId,
            moduleId,
            projectId,
            workspaceId
        });
    }

    const erdFormElement = document.getElementById(erdFormId);
    const erdListContainer = document.getElementById(erdListId);

    initializeErdDefinitionSection({
        moduleId,
        formElement: erdFormElement,
        listContainer: erdListContainer,
        isErdTask,
        formWrapperId: erdFormWrapperId
    });
}

function buildErdDefinitionPayload(context) {
    const { formElement, moduleId } = context;
    if (!formElement || !moduleId) {
        return null;
    }

    const formData = new FormData(formElement);
    const tableName = formData.get('tName')?.toString().trim() ?? '';
    if (!tableName) {
        showNotification('Table name is required.', 'warning');
        return null;
    }

    const sortOrderValue = formData.get('sortOrder');
    const sortOrder = sortOrderValue !== null && sortOrderValue !== undefined
        ? Number.parseInt(sortOrderValue.toString(), 10) || 1
        : 1;

    const attributes = collectAttributeRows(context);
    if (attributes.length === 0) {
        showNotification('Add at least one attribute before saving an ERD definition.', 'warning');
        return null;
    }

    const hasAttributeWithoutName = attributes.some(attribute => !attribute.name);
    if (hasAttributeWithoutName) {
        showNotification('Each attribute must include a name.', 'warning');
        return null;
    }

    return {
        ModuleId: moduleId,
        TName: tableName,
        Description: formData.get('description')?.toString().trim() ?? '',
        EntityName: formData.get('entityName')?.toString().trim() ?? '',
        SortOrder: sortOrder,
        Attributes: attributes
    };
}

function resetErdDefinitionForm(context) {
    const { formElement, isErdTask } = context;
    if (!formElement) {
        return;
    }

    formElement.reset();
    context.state.editingId = null;

    populateAttributeRows(context, []);

    const submitButton = formElement.querySelector('[data-role="erd-submit"]');
    if (submitButton) {
        submitButton.textContent = 'Save ERD Definition';
        submitButton.disabled = !isErdTask;
    }

    const cancelButton = formElement.querySelector('[data-action="cancel-erd-edit"]');
    if (cancelButton) {
        cancelButton.classList.add('d-none');
    }
}

function startEditErdDefinition(context, erdId) {
    const { formElement, state } = context;
    if (!formElement || !state) {
        return;
    }

    const definition = (state.definitions || []).find(item => (item?.id ?? item?.Id) === erdId);
    if (!definition) {
        showNotification('Unable to load ERD definition for editing.', 'warning');
        return;
    }

    state.editingId = erdId;
    const resolveValue = (value) => value ?? '';

    const tNameInput = formElement.querySelector('input[name="tName"]');
    if (tNameInput) {
        tNameInput.value = resolveValue(definition.tName ?? definition.TName);
    }

    const entityNameInput = formElement.querySelector('input[name="entityName"]');
    if (entityNameInput) {
        entityNameInput.value = resolveValue(definition.entityName ?? definition.EntityName);
    }

    const descriptionInput = formElement.querySelector('textarea[name="description"]');
    if (descriptionInput) {
        descriptionInput.value = resolveValue(definition.description ?? definition.Description);
    }

    const sortOrderInput = formElement.querySelector('input[name="sortOrder"]');
    if (sortOrderInput) {
        sortOrderInput.value = resolveValue(definition.sortOrder ?? definition.SortOrder ?? 1);
    }

    const attributes = definition.attributes ?? definition.Attributes ?? [];
    populateAttributeRows(context, attributes);

    const submitButton = formElement.querySelector('[data-role="erd-submit"]');
    if (submitButton) {
        submitButton.textContent = 'Update ERD Definition';
        submitButton.disabled = !context.isErdTask;
    }

    const cancelButton = formElement.querySelector('[data-action="cancel-erd-edit"]');
    if (cancelButton) {
        cancelButton.classList.remove('d-none');
    }

    formElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function deleteErdDefinition(context, erdId) {
    const { moduleId, state } = context;
    if (!moduleId || !erdId) {
        return;
    }

    if (!confirm('Are you sure you want to delete this ERD definition?')) {
        return;
    }

    fetch(`/api/modules/${moduleId}/erd-definitions/${erdId}`, { method: 'DELETE' })
        .then(response => {
            if (response.ok) {
                showNotification('ERD definition deleted successfully.', 'success');
                if (state.editingId === erdId) {
                    resetErdDefinitionForm(context);
                }
                loadErdDefinitions(context);
            } else if (response.status === 404) {
                throw new Error('ERD definition not found');
            } else {
                throw new Error('Failed to delete ERD definition');
            }
        })
        .catch(error => {
            console.error('Error deleting ERD definition:', error);
            showNotification(`Error deleting ERD definition: ${error.message}`, 'danger');
        });
}

function renderErdDefinitionsList(context) {
    const { listContainer, state, isErdTask } = context;
    if (!listContainer || !state) {
        return;
    }

    const definitions = state.definitions ?? [];
    if (definitions.length === 0) {
        listContainer.innerHTML = '<div class="alert alert-info mb-0">No ERD definitions found for this module.</div>';
        return;
    }

    const cards = definitions
        .slice()
        .sort((a, b) => (a.sortOrder ?? a.SortOrder ?? 0) - (b.sortOrder ?? b.SortOrder ?? 0))
        .map(definition => {
            const definitionId = definition?.id ?? definition?.Id;
            const description = definition?.description ?? definition?.Description;
            const entityName = definition?.entityName ?? definition?.EntityName;
            const tName = definition?.tName ?? definition?.TName;
            const sortOrder = definition?.sortOrder ?? definition?.SortOrder ?? 0;
            const statusValue = definition?.erdStatus ?? definition?.ErdStatus;

            const attributes = Array.isArray(definition?.attributes)
                ? definition.attributes
                : Array.isArray(definition?.Attributes)
                    ? definition.Attributes
                    : [];

            const attributeItems = attributes.length > 0
                ? attributes
                    .slice()
                    .sort((a, b) => (a.sortOrder ?? a.SortOrder ?? 0) - (b.sortOrder ?? b.SortOrder ?? 0))
                    .map(attribute => {
                        const attributeName = attribute?.name ?? attribute?.Name ?? 'Unnamed Attribute';
                        const dataType = attribute?.dataType ?? attribute?.DataType ?? '-';
                        const attributeDescription = attribute?.description ?? attribute?.Description ?? '';
                        const maxChar = attribute?.maxChar ?? attribute?.MaxChar;
                        const attributeSortOrder = attribute?.sortOrder ?? attribute?.SortOrder;
                        const isPrimary = Boolean(attribute?.isPrimary ?? attribute?.IsPrimary);
                        const isNull = Boolean(attribute?.isNull ?? attribute?.IsNull);
                        const isForeignKey = Boolean(attribute?.isForeignKey ?? attribute?.IsForeignKey);
                        const foreignKeyTable = attribute?.foreignKeyTable ?? attribute?.ForeignKeyTable ?? '';

                        return `
                            <li class="list-group-item">
                                <div class="d-flex justify-content-between align-items-start gap-3">
                                    <div>
                                        <div class="fw-semibold">${escapeHtml(attributeName)}</div>
                                        <div class="small text-muted">${escapeHtml(dataType)}</div>
                                        ${attributeDescription ? `<div class="small text-muted">${escapeHtml(attributeDescription)}</div>` : ''}
                                        <div class="small text-muted">
                                            <span><strong>Sort:</strong> ${escapeHtml(attributeSortOrder ?? '-')}</span>
                                            <span class="ms-2"><strong>Max Char:</strong> ${escapeHtml(maxChar ?? '-')}</span>
                                        </div>
                                        <div class="small text-muted mt-1">
                                            <span class="me-2"><strong>Primary:</strong> ${isPrimary ? 'Yes' : 'No'}</span>
                                            <span class="me-2"><strong>Nullable:</strong> ${isNull ? 'Yes' : 'No'}</span>
                                            <span><strong>Foreign Key:</strong> ${isForeignKey ? `Yes (${escapeHtml(foreignKeyTable || '-')})` : 'No'}</span>
                                        </div>
                                    </div>
                                </div>
                            </li>
                        `;
                    })
                    .join('')
                : '<li class="list-group-item text-muted small">No attributes defined.</li>';

            return `
                <div class="card shadow-sm mb-3">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start gap-3">
                            <div>
                                <h6 class="mb-1">${escapeHtml(tName || 'Untitled Definition')}</h6>
                                <p class="text-muted small mb-2">${description ? escapeHtml(description) : 'No description provided.'}</p>
                                <div class="small text-muted">
                                    <div><strong>Entity:</strong> ${escapeHtml(entityName || '-')}</div>
                                    <div><strong>Sort Order:</strong> ${sortOrder}</div>
                                    <div><strong>Status:</strong> ${escapeHtml(statusValue ?? 'N/A')}</div>
                                </div>
                            </div>
                            <div class="btn-group btn-group-sm flex-shrink-0">
                                <button class="btn btn-outline-primary-sm" data-action="edit-erd" data-definition-id="${definitionId}" ${!isErdTask ? 'disabled' : ''}>
                                    Edit
                                </button>
                                <button class="btn btn-outline-danger-sm" data-action="delete-erd" data-definition-id="${definitionId}" ${!isErdTask ? 'disabled' : ''}>
                                    Delete
                                </button>
                            </div>
                        </div>
                        <div class="mt-3">
                            <h6 class="small text-muted mb-2">Attributes</h6>
                            <div class="card border-0">
                                <ul class="list-group list-group-flush">
                                    ${attributeItems}
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        })
        .join('');

    listContainer.innerHTML = cards;

    listContainer.querySelectorAll('[data-action="edit-erd"]').forEach(button => {
        button.addEventListener('click', () => {
            const definitionId = button.getAttribute('data-definition-id');
            startEditErdDefinition(context, definitionId);
        });
    });

    listContainer.querySelectorAll('[data-action="delete-erd"]').forEach(button => {
        button.addEventListener('click', () => {
            const definitionId = button.getAttribute('data-definition-id');
            deleteErdDefinition(context, definitionId);
        });
    });
}

function loadErdDefinitions(context) {
    const { moduleId, listContainer, state, formElement, isErdTask } = context;
    if (!moduleId || !state) {
        if (listContainer) {
            listContainer.innerHTML = '<div class="alert alert-warning mb-0">Module information is required to load ERD definitions.</div>';
        }
        if (formElement) {
            Array.from(formElement.elements || []).forEach(el => { el.disabled = true; });
            const submitButton = formElement.querySelector('[data-role="erd-submit"]');
            if (submitButton) {
                submitButton.disabled = true;
            }
        }
        return;
    }

    fetch(`/api/modules/${moduleId}/erd-definitions`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(definitions => {
            state.definitions = Array.isArray(definitions) ? definitions : [];
            renderErdDefinitionsList(context);
            if (formElement) {
                Array.from(formElement.elements || []).forEach(el => {
                    if (el.type !== 'button' && el.type !== 'submit') {
                        el.disabled = !isErdTask;
                    }
                });
                const submitButton = formElement.querySelector('[data-role="erd-submit"]');
                if (submitButton) {
                    submitButton.disabled = !isErdTask;
                }
            }

            updateAttributeRowState(context);
        })
        .catch(error => {
            console.error('Error loading ERD definitions:', error);
            if (listContainer) {
                listContainer.innerHTML = `<div class="alert alert-danger mb-0">Error loading ERD definitions: ${escapeHtml(error.message)}</div>`;
            }
            updateAttributeRowState(context);
        });
}

function initializeErdDefinitionSection(context) {
    const { formElement, isErdTask } = context;
    context.state = { editingId: null, definitions: [] };
    context.isErdTask = isErdTask;

    if (formElement) {
        context.formElement = formElement;
        context.formId = formElement.id || `erd-form-${Date.now()}`;
        context.attributeRowsContainer = formElement.querySelector('[data-role="attribute-rows"]') || null;
        context.addAttributeButton = formElement.querySelector('[data-action="add-attribute-row"]') || null;

        populateAttributeRows(context, []);

        if (context.addAttributeButton) {
            context.addAttributeButton.addEventListener('click', () => {
                if (!context.isErdTask) {
                    return;
                }
                addAttributeRow(context, null);
            });
        }

        const submitButton = formElement.querySelector('[data-role="erd-submit"]');
        if (submitButton && !context.isErdTask) {
            submitButton.disabled = true;
        }

        const cancelButton = formElement.querySelector('[data-action="cancel-erd-edit"]');
        if (cancelButton) {
            cancelButton.addEventListener('click', () => {
                resetErdDefinitionForm(context);
            });
        }

        formElement.addEventListener('submit', function (event) {
            event.preventDefault();

            if (!context.isErdTask) {
                showNotification('Task is not marked as ERD. Enable ERD to modify definitions.', 'warning');
                return;
            }

            const payload = buildErdDefinitionPayload(context);
            if (!payload) {
                return;
            }

            const isUpdate = Boolean(context.state.editingId);
            const url = isUpdate
                ? `/api/modules/${context.moduleId}/erd-definitions/${context.state.editingId}`
                : `/api/modules/${context.moduleId}/erd-definitions`;
            const method = isUpdate ? 'PUT' : 'POST';

            fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => {
                            const message = text || 'Failed to save ERD definition';
                            throw new Error(message);
                        });
                    }
                    return response.json();
                })
                .then(() => {
                    showNotification(isUpdate ? 'ERD definition updated successfully.' : 'ERD definition created successfully.', 'success');
                    resetErdDefinitionForm(context);
                    loadErdDefinitions(context);
                })
                .catch(error => {
                    console.error('Error saving ERD definition:', error);
                    showNotification(`Error saving ERD definition: ${error.message}`, 'danger');
                });
        });
    }

    loadErdDefinitions(context);
}

// Fungsi untuk melihat detail task
function viewTaskDetails(taskId) {
    console.log('View task details for ID:', taskId);
    
    // Dapatkan moduleId dari elemen DOM terkait
    const taskElement = document.querySelector(`[data-task-id="${taskId}"]`);
    const moduleId = taskElement?.closest('[id^="tasks-"]')?.id?.replace('tasks-', '');
    
    if (!moduleId) {
        showNotification('Could not determine module ID.', 'danger');
        return;
    }
    
    // Muat data task
    fetch(`/api/modules/${moduleId}/tasks`)
        .then(response => response.json())
        .then(tasks => {
            const task = tasks.find(t => t.id === taskId);
            if (task) {
                renderTaskDetail(task, moduleId);
            } else {
                showNotification('Task not found.', 'warning');
            }
        })
        .catch(error => {
            console.error('Error loading task details:', error);
            showNotification(`Error loading task details: ${error.message}`, 'danger');
        });
}

// Fungsi untuk memuat modal edit task
function loadEditTaskModal(taskId) {
    console.log('Load edit modal for task ID:', taskId);
    
    // Dapatkan moduleId dari elemen DOM terkait
    const taskElement = document.querySelector(`[data-task-id="${taskId}"]`);
    const moduleId = taskElement?.closest('[id^="tasks-"]')?.id?.replace('tasks-', '');
    
    if (!moduleId) {
        showNotification('Could not determine module ID.', 'danger');
        return;
    }
    
    fetch(`/TaskView/EditModal?moduleId=${moduleId}&taskId=${taskId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            // Buat elemen modal container jika belum ada
            let modalContainer = document.getElementById('editTaskModalContainer');
            if (!modalContainer) {
                modalContainer = document.createElement('div');
                modalContainer.id = 'editTaskModalContainer';
                document.body.appendChild(modalContainer);
            }
            
            // Isi modal container dengan HTML dari response
            modalContainer.innerHTML = html;
            
            // Tampilkan modal menggunakan Bootstrap
            const modalElement = document.getElementById('editTaskModal');
            if (modalElement) {
                // Tampilkan modal
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
                
                // Tangani event saat modal disembunyikan
                modalElement.addEventListener('hidden.bs.modal', function () {
                    // Bersihkan modal setelah ditutup
                    cleanupModal('editTaskModalContainer');
                });
                
                // Isi form dengan data task yang ada
                fetch(`/api/modules/${moduleId}/tasks`)
                    .then(response => response.json())
                    .then(tasks => {
                        const task = tasks.find(t => t.id === taskId);
                        if (task) {
                            const isErd = resolveTaskIsErd(task);
                            document.getElementById('editName').value = task.name || '';
                            document.getElementById('editDescription').value = task.description || '';
                            document.getElementById('editSortOrder').value = task.sortOrder || 1;
                            document.getElementById('editIsErd').checked = isErd;
                            
                            // Tampilkan tombol ERD Definition jika task adalah ERD
                            const erdDefinitionButton = document.getElementById('editErdDefinitionButton');
                            if (erdDefinitionButton) {
                                if (isErd) {
                                    erdDefinitionButton.style.display = 'block';
                                } else {
                                    erdDefinitionButton.style.display = 'none';
                                }
                            }
                        }
                    })
                    .catch(error => console.error('Error loading task data:', error));
                
                // Tangani form submission
                const form = document.getElementById('editTaskForm');
                if (form) {
                    form.addEventListener('submit', function (e) {
                        e.preventDefault();
                        
                        const formData = new FormData(this);
                        const isErdValue = this.querySelector('input[name="isErd"]')?.checked || false;
                        
                        const requestData = {
                            Name: formData.get('name'),
                            Description: formData.get('description'),
                            IsErd: isErdValue,
                            SortOrder: parseInt(formData.get('sortOrder')) || 1
                        };
                        
                        fetch(`/api/modules/${moduleId}/tasks/${taskId}`, {
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
                            // Sembunyikan modal
                            const modalInstance = bootstrap.Modal.getInstance(modalElement);
                            if (modalInstance) {
                                modalInstance.hide();
                            } else {
                                // Fallback jika instance tidak ditemukan
                                modalElement.classList.remove('show');
                                modalElement.style.display = 'none';
                                document.body.classList.remove('modal-open');
                            }
                            const backdrop = document.querySelector('.modal-backdrop');
                            if (backdrop) {
                                backdrop.remove();
                            }

                            // Reload tasks
                            loadTasks(moduleId);
                            
                            showNotification('Task updated successfully.', 'success');
                        })
                        .catch(error => {
                            showNotification(`Error updating task: ${error.message}`, 'danger');
                            console.error('Error:', error);
                        });
                    });
                }
            }
        })
        .catch(error => {
            console.error('Error loading edit task modal:', error);
            showNotification('Error loading edit task modal.', 'danger');
        });
}

// Fungsi untuk menghapus task
function deleteTask(taskId) {
    // Konfirmasi sebelum menghapus
    if (!confirm('Are you sure you want to delete this task? All flow tasks and ERD definitions will also be deleted.')) {
        return;
    }

    // Dapatkan moduleId dari elemen DOM terkait
    const taskElement = document.querySelector(`[data-task-id="${taskId}"]`);
    const moduleId = taskElement?.closest('[id^="tasks-"]')?.id?.replace('tasks-', '');
    
    if (!moduleId) {
        showNotification('Could not determine module ID.', 'danger');
        return;
    }

    // Periksa apakah ada flow task dengan status In Progress
    fetch(`/api/tasks/${taskId}/flow-tasks`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(flowTasks => {
            // Periksa apakah ada flow task dengan status In Progress (0 - sesuai dengan enum FlowStatus.InProgress)
            const inProgressFlowTasks = flowTasks.filter(ft => ft.flowStatus === 0);
            
            if (inProgressFlowTasks.length > 0) {
                showNotification('Cannot delete task. There are flow tasks in progress.', 'warning');
                return;
            }
            
            // Jika tidak ada flow task dengan status In Progress, lanjutkan penghapusan
            return fetch(`/api/modules/${moduleId}/tasks/${taskId}`, {
                method: 'DELETE'
            });
        })
        .then(response => {
            if (response && response.ok) {
                // Muat ulang daftar task setelah penghapusan berhasil
                loadTasks(moduleId);
                showNotification('Task deleted successfully.', 'success');
            } else if (response && response.status === 404) {
                throw new Error('Task not found');
            } else if (response) {
                throw new Error('Failed to delete task');
            }
        })
        .catch(error => {
            console.error('Error deleting task:', error);
            showNotification(`Error deleting task: ${error.message}`, 'danger');
        });
}

// Fungsi untuk menambahkan ERD definition
function addErdDefinition(taskId) {
    if (!taskId) {
        return;
    }

    viewTaskDetails(taskId);

    setTimeout(() => {
        const collapseElement = document.getElementById(`task-detail-erd-${taskId}`);
        if (collapseElement && typeof bootstrap !== 'undefined') {
            const collapseInstance = bootstrap.Collapse.getOrCreateInstance(collapseElement, { toggle: false });
            collapseInstance.show();
        }
    }, 350);
}

// Fungsi untuk memuat daftar task
function loadTasks(moduleId) {
    fetch(`/api/modules/${moduleId}/tasks`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            const tasksContainer = document.getElementById(`tasks-${moduleId}`);
            if (tasksContainer) {
                const tasksById = new Map();

                let tasksHtml = `
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h6>Tasks</h6>
                        <button class="btn btn-outline-success-sm create-task-btn" data-module-id="${moduleId}">
                            <i class="fas fa-plus"></i> New Task
                        </button>
                    </div>
                `;
                
                if (data && data.length > 0) {
                    tasksHtml += '<div class="row">';
                    data.forEach(function (task) {
                        const taskIdValue = task.id ?? task.Id;
                        if (taskIdValue) {
                            tasksById.set(taskIdValue, task);
                        }
                        const isErd = resolveTaskIsErd(task);
                        const moduleIdValue = task.moduleId ?? task.ModuleId ?? moduleId;
                        const projectIdValue = task.projectId ?? task.ProjectId ?? '';
                        const workspaceIdValue = task.workspaceId ?? task.WorkspaceId ?? '';
                        const flowTasksContainerId = `flow-task-list-${taskIdValue}`;

                        const erdDefinitionButton = isErd ?
                            `<button class="btn btn-outline-purple-sm add-erd-definition-btn" data-task-id="${taskIdValue}">+ ERD Definition</button>` :
                            '';

                        tasksHtml += `
                            <div class="col-md-12 mb-2">
                                <div class="card">
                                    <div class="card-body">
                                        <div class="d-flex justify-content-between align-items-start flex-wrap gap-2">
                                            <div>
                                                <h6 class="card-title mb-1">${task.name || 'No name'}</h6>
                                                <span class="badge bg-${isErd ? 'success' : 'danger'}">${isErd ? 'ERD' : 'Non-ERD'}</span>
                                            </div>
                                            <div class="d-flex flex-wrap gap-2">
                                                <button class="btn btn-outline-info-sm view-task-btn" data-task-id="${taskIdValue}">View</button>
                                                <button class="btn btn-outline-primary-sm edit-task-btn" data-task-id="${taskIdValue}">Edit</button>
                                                <button class="btn btn-outline-danger-sm delete-task-btn" data-task-id="${taskIdValue}">Delete</button>
                                                <button class="btn btn-outline-secondary-sm add-flow-task-btn"
                                                        data-task-id="${taskIdValue}"
                                                        data-module-id="${moduleIdValue}"
                                                        data-project-id="${projectIdValue}"
                                                        data-workspace-id="${workspaceIdValue}"
                                                        data-container-id="${flowTasksContainerId}">
                                                    Add Flow Task
                                                </button>
                                                ${erdDefinitionButton}
                                            </div>
                                        </div>
                                        <p class="card-text mt-2 mb-3">${truncateText(task.description || 'No description', 100)}</p>
                                        <div class="flow-tasks-container border-top pt-3 mt-3"
                                             id="${flowTasksContainerId}"
                                             data-task-id="${taskIdValue}"
                                             data-module-id="${moduleIdValue}"
                                             data-project-id="${projectIdValue}"
                                             data-workspace-id="${workspaceIdValue}">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
                    });
                    tasksHtml += '</div>';
                } else {
                    tasksHtml += '<p>No tasks found.</p>';
                }
                
                tasksContainer.innerHTML = tasksHtml;
                
                // Attach event handlers for create task buttons
                tasksContainer.querySelectorAll('.create-task-btn').forEach(button => {
                    // Hindari duplikasi event listener
                    const newButton = button.cloneNode(true);
                    button.parentNode.replaceChild(newButton, button);
                    newButton.addEventListener('click', function () {
                        const moduleId = this.getAttribute('data-module-id');
                        loadCreateTaskModal(moduleId);
                    });
                });
                
                // Attach event handlers for view task buttons
                tasksContainer.querySelectorAll('.view-task-btn').forEach(button => {
                    // Hindari duplikasi event listener
                    const newButton = button.cloneNode(true);
                    button.parentNode.replaceChild(newButton, button);
                    newButton.addEventListener('click', function () {
                        const taskId = this.getAttribute('data-task-id');
                        const taskData = tasksById.get(taskId);
                        if (taskData) {
                            renderTaskDetail(taskData, moduleId);
                        } else {
                            viewTaskDetails(taskId);
                        }
                    });
                });
                
                // Attach event handlers for edit task buttons
                tasksContainer.querySelectorAll('.edit-task-btn').forEach(button => {
                    // Hindari duplikasi event listener
                    const newButton = button.cloneNode(true);
                    button.parentNode.replaceChild(newButton, button);
                    newButton.addEventListener('click', function () {
                        const taskId = this.getAttribute('data-task-id');
                        loadEditTaskModal(taskId);
                    });
                });
                
                // Attach event handlers for delete task buttons
                tasksContainer.querySelectorAll('.delete-task-btn').forEach(button => {
                    // Hindari duplikasi event listener
                    const newButton = button.cloneNode(true);
                    button.parentNode.replaceChild(newButton, button);
                    newButton.addEventListener('click', function () {
                        const taskId = this.getAttribute('data-task-id');
                        deleteTask(taskId);
                    });
                });
                
                // Attach event handlers for add flow task buttons
                tasksContainer.querySelectorAll('.add-flow-task-btn').forEach(button => {
                    // Hindari duplikasi event listener
                    const newButton = button.cloneNode(true);
                    button.parentNode.replaceChild(newButton, button);
                    newButton.addEventListener('click', function () {
                        const taskId = this.getAttribute('data-task-id');
                        const moduleIdValue = this.getAttribute('data-module-id');
                        const projectIdValue = this.getAttribute('data-project-id');
                        const workspaceIdValue = this.getAttribute('data-workspace-id');
                        const containerId = this.getAttribute('data-container-id');
                        if (window.flowTaskManager) {
                            window.flowTaskManager.openCreateModal({
                                taskId,
                                moduleId: moduleIdValue,
                                projectId: projectIdValue,
                                workspaceId: workspaceIdValue,
                                containerId
                            });
                        } else {
                            console.error('flowTaskManager is not available.');
                        }
                    });
                });
                
                // Attach event handlers for add ERD definition buttons
                // Attach event handlers for add ERD definition buttons
                tasksContainer.querySelectorAll('.add-erd-definition-btn').forEach(button => {
                    // Hindari duplikasi event listener
                    const newButton = button.cloneNode(true);
                    button.parentNode.replaceChild(newButton, button);
                    newButton.addEventListener('click', function () {
                        const taskId = this.getAttribute('data-task-id');
                        const taskData = tasksById.get(taskId);
                        if (taskData) {
                            renderTaskDetail(taskData, moduleId);
                            const collapseElement = document.getElementById(`task-detail-erd-${taskId}`);
                            if (collapseElement) {
                                const collapseInstance = bootstrap.Collapse.getOrCreateInstance(collapseElement, { toggle: false });
                                collapseInstance.show();
                            }
                        } else {
                            viewTaskDetails(taskId);
                        }
                    });
                });

                if (window.flowTaskManager) {
                    tasksContainer.querySelectorAll('.flow-tasks-container').forEach(container => {
                        const { taskId, moduleId, projectId, workspaceId } = container.dataset;
                        window.flowTaskManager.refreshFlowTasks({
                            containerId: container.id,
                            taskId,
                            moduleId,
                            projectId,
                            workspaceId
                        });
                    });
                }

                const detailContainer = document.getElementById('detail-of-task');
                if (detailContainer) {
                    const selectedTaskId = detailContainer.dataset.currentTaskId;
                    if (selectedTaskId) {
                        const selectedTask = tasksById.get(selectedTaskId);
                        if (selectedTask) {
                            renderTaskDetail(selectedTask, moduleId);
                        } else {
                            detailContainer.innerHTML = `
                                <div class="alert alert-info shadow-sm mb-0">
                                    Selected task is no longer available. Choose another task to view details.
                                </div>
                            `;
                            delete detailContainer.dataset.currentTaskId;
                            delete detailContainer.dataset.currentModuleId;
                        }
                    } else {
                        ensureTaskDetailPlaceholder();
                    }
                }
            }
        })
        .catch(error => {
            console.error('Error loading tasks:', error);
            const tasksContainer = document.getElementById(`tasks-${moduleId}`);
            if (tasksContainer) {
                tasksContainer.innerHTML = '<p>Error loading tasks. Please try again later.</p>';
            }

            ensureTaskDetailPlaceholder();
        });
}

// Fungsi untuk memuat modal pembuatan task
function loadCreateTaskModal(moduleId) {
    fetch(`/TaskView/CreateModal?moduleId=${moduleId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            // Buat elemen modal container jika belum ada
            let modalContainer = document.getElementById('createTaskModalContainer');
            if (!modalContainer) {
                modalContainer = document.createElement('div');
                modalContainer.id = 'createTaskModalContainer';
                document.body.appendChild(modalContainer);
            }
            
            // Isi modal container dengan HTML dari response
            modalContainer.innerHTML = html;
            
            // Tampilkan modal menggunakan Bootstrap
            const modalElement = document.getElementById('createTaskModal');
            if (modalElement) {
                // Tampilkan modal
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
                
                // Tangani event saat modal disembunyikan
                modalElement.addEventListener('hidden.bs.modal', function () {
                    // Bersihkan modal setelah ditutup
                    cleanupModal('createTaskModalContainer');
                });
                
                // Setup event listener untuk checkbox IsERD
                const isErdCheckbox = document.getElementById('isErd');
                const erdDefinitionButton = document.getElementById('erdDefinitionButton');
                if (isErdCheckbox && erdDefinitionButton) {
                    isErdCheckbox.addEventListener('change', function () {
                        if (this.checked) {
                            erdDefinitionButton.style.display = 'block';
                        } else {
                            erdDefinitionButton.style.display = 'none';
                        }
                    });
                }
                
                // Tangani form submission
                const form = document.getElementById('createTaskForm');
                if (form) {
                    form.addEventListener('submit', function (e) {
                        e.preventDefault();
                        
                        const formData = new FormData(this);
                        const isErdValue = this.querySelector('input[name="isErd"]')?.checked || false;
                        
                        const requestData = {
                            ModuleId: formData.get('moduleId'),
                            Name: formData.get('name'),
                            Description: formData.get('description'),
                            IsErd: isErdValue,
                            SortOrder: parseInt(formData.get('sortOrder')) || 1
                        };
                        
                        fetch(`/api/modules/${moduleId}/tasks`, {
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
                            // Sembunyikan modal
                            const modalInstance = bootstrap.Modal.getInstance(modalElement);
                            if (modalInstance) {
                                modalInstance.hide();
                            } else {
                                // Fallback jika instance tidak ditemukan
                                modalElement.classList.remove('show');
                                modalElement.style.display = 'none';
                                document.body.classList.remove('modal-open');
                            }
                            const backdrop = document.querySelector('.modal-backdrop');
                            if (backdrop) {
                                backdrop.remove();
                            }

                            // Reload tasks
                            loadTasks(moduleId);
                            
                            showNotification('Task created successfully.', 'success');
                        })
                        .catch(error => {
                            showNotification(`Error creating task: ${error.message}`, 'danger');
                            console.error('Error:', error);
                        });
                    });
                }
            }
        })
        .catch(error => {
            console.error('Error loading create task modal:', error);
            showNotification('Error loading create task modal.', 'danger');
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

document.addEventListener('DOMContentLoaded', ensureTaskDetailPlaceholder);
