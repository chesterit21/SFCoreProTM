
import * as erdApi from '../services/erdApiService.js';
import * as erdTpl from '../templates/erdTemplates.js';
import { showNotification } from '../utils/notifications.js';

function getFormPayload(form, context) {
    const formData = new FormData(form);
    const tableName = formData.get('tName')?.trim();
    if (!tableName) {
        showNotification('Table name is required.', 'warning');
        return null;
    }

    return {
        ModuleId: context.moduleId,
        TName: tableName,
        Description: formData.get('description')?.trim() ?? '',
        EntityName: formData.get('entityName')?.trim() ?? '',
        Attributes: context.state.attributes
    };
}

function updateAttributeSortOrder(attributes) {
    return attributes.map((attr, index) => ({ ...attr, sortOrder: index + 1 }));
}

function handleAttributeDragDrop(context) {
    const tbody = context.attributeListContainer.querySelector('tbody');
    if (!tbody) return;

    let draggedId = null;

    tbody.addEventListener('dragstart', e => {
        draggedId = e.target.dataset.attributeId;
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', draggedId);
        e.target.classList.add('dragging');
    });

    tbody.addEventListener('dragend', e => {
        e.target.classList.remove('dragging');
        const draggedOver = tbody.querySelector('.drag-over');
        if (draggedOver) {
            draggedOver.classList.remove('drag-over');
        }
    });

    tbody.addEventListener('dragover', e => {
        e.preventDefault();
        const targetRow = e.target.closest('tr');
        if (targetRow && targetRow.dataset.attributeId !== draggedId) {
            const allRows = [...tbody.querySelectorAll('tr')];
            allRows.forEach(row => row.classList.remove('drag-over'));
            targetRow.classList.add('drag-over');
        }
    });

    tbody.addEventListener('dragleave', e => {
        const targetRow = e.target.closest('tr');
        if (targetRow) {
            targetRow.classList.remove('drag-over');
        }
    });

    tbody.addEventListener('drop', e => {
        e.preventDefault();
        const fromId = e.dataTransfer.getData('text/plain');
        const toRow = e.target.closest('tr');
        if (!toRow) return;

        const toId = toRow.dataset.attributeId;
        toRow.classList.remove('drag-over');

        if (fromId === toId) return;

        let attrs = context.state.attributes;
        const fromIndex = attrs.findIndex(a => (a.id ?? a.Id ?? `temp-${a.tempId}`) === fromId);
        const toIndex = attrs.findIndex(a => (a.id ?? a.Id ?? `temp-${a.tempId}`) === toId);

        if (fromIndex === -1 || toIndex === -1) return;

        const [movedItem] = attrs.splice(fromIndex, 1);
        attrs.splice(toIndex, 0, movedItem);

        context.state.attributes = updateAttributeSortOrder(attrs);
        erdTpl.renderAttributeTable(context.attributeListContainer, context.state.attributes, context);
    });
}


function populateFormForEdit(context, definition) {
    const { form, modal } = context;
    context.state.editingId = definition.id ?? definition.Id;

    form.querySelector('[name="tName"]').value = definition.tName || '';
    form.querySelector('[name="entityName"]').value = definition.entityName || '';
    form.querySelector('[name="description"]').value = definition.description || '';

    context.state.attributes = updateAttributeSortOrder([...(definition.attributes || definition.Attributes || [])]);
    erdTpl.renderAttributeTable(context.attributeListContainer, context.state.attributes, context);

    form.closest('.modal').querySelector('.modal-title').textContent = 'Edit ERD Definition';
    modal.show();
}

function resetForm(context) {
    const { form } = context;
    context.state.editingId = null;
    context.state.attributes = [];
    form.reset();
    context.modalElement.querySelector(`#attribute-form-${context.moduleId}`).reset();
    erdTpl.renderAttributeTable(context.attributeListContainer, [], context);
    form.closest('.modal').querySelector('.modal-title').textContent = 'Add ERD Definition';
}

async function handleSaveErd(context) {
    const payload = getFormPayload(context.form, context);
    if (!payload) return;

    try {
        if (context.state.editingId) {
            await erdApi.updateErdDefinition(context.moduleId, context.state.editingId, payload);
            showNotification('ERD definition updated successfully.', 'success');
        } else {
            await erdApi.createErdDefinition(context.moduleId, payload);
            showNotification('ERD definition created successfully.', 'success');
        }
        await refreshErdDefinitions(context);
        context.modal.hide();
    } catch (error) {
        showNotification(`Error saving ERD definition: ${error.message}`, 'danger');
    }
}

async function refreshErdDefinitions(context) {
    const definitions = await erdApi.getErdDefinitions(context.moduleId);
    context.state.definitions = definitions;
    erdTpl.renderErdDefinitionsTable(context.listContainer, definitions, context);
}

function attachErdListeners(context) {
    const mainContainer = document.getElementById(context.containerId);

    mainContainer.addEventListener('click', (e) => {
        const target = e.target.closest('[data-action]');
        if (!target) return;

        const action = target.dataset.action;

        if (action === 'add-erd') {
            resetForm(context);
            context.modal.show();
        }

        if (action === 'edit-erd') {
            const defId = target.dataset.definitionId;
            const definition = context.state.definitions.find(d => (d.id ?? d.Id) === defId);
            if (definition) {
                populateFormForEdit(context, definition);
            }
        }

        if (action === 'delete-erd') {
            const defId = target.dataset.definitionId;
            if (confirm('Are you sure you want to delete this ERD definition?')) {
                erdApi.deleteErdDefinition(context.moduleId, defId).then(() => {
                    showNotification('ERD definition deleted.', 'success');
                    refreshErdDefinitions(context);
                }).catch(err => showNotification(err.message, 'danger'));
            }
        }
    });

    context.modalElement.addEventListener('click', e => {
        const target = e.target.closest('[data-action]');
        if (!target) return;

        const action = target.dataset.action;

        if (action === 'add-attribute-to-list') {
            const form = context.modalElement.querySelector(`#attribute-form-${context.moduleId}`);
            const name = form.querySelector('[data-field="name"]').value.trim();
            if (!name) {
                showNotification('Attribute Name is required.', 'warning');
                return;
            }
            const newAttr = {
                tempId: Date.now(),
                name: name,
                dataType: form.querySelector('[data-field="dataType"]').value.trim(),
                description: form.querySelector('[data-field="description"]').value.trim(),
                maxChar: parseInt(form.querySelector('[data-field="maxChar"]').value, 10) || null,
                isPrimary: form.querySelector('[data-field="isPrimary"]').checked,
                isNull: form.querySelector('[data-field="isNull"]').checked,
                isForeignKey: form.querySelector('[data-field="isForeignKey"]').checked,
                foreignKeyTable: form.querySelector('[data-field="foreignKeyTable"]').value.trim(),
                sortOrder: context.state.attributes.length + 1
            };
            context.state.attributes.push(newAttr);
            context.state.attributes = updateAttributeSortOrder(context.state.attributes);
            erdTpl.renderAttributeTable(context.attributeListContainer, context.state.attributes, context);
            form.reset();
        }

        if (action === 'delete-attribute') {
            const attrIdToDelete = target.dataset.attributeId;
            let attrs = context.state.attributes.filter(attr => (attr.id ?? attr.Id ?? `temp-${attr.tempId}`) !== attrIdToDelete);
            context.state.attributes = updateAttributeSortOrder(attrs);
            erdTpl.renderAttributeTable(context.attributeListContainer, context.state.attributes, context);
        }
    });

    context.modalElement.querySelector('[data-role="erd-submit"]').addEventListener('click', () => {
        handleSaveErd(context);
    });

    handleAttributeDragDrop(context);
}

export async function initErdSection(context) {
    const container = document.getElementById(context.containerId);
    if (!container) return;

    erdTpl.renderErdSectionLayout(container, context);

    const modalElement = container.querySelector(`#erd-modal-${context.moduleId}`);
    const modal = new bootstrap.Modal(modalElement);

    const handlerContext = {
        ...context,
        form: container.querySelector(`#erd-definition-form-${context.moduleId}`),
        listContainer: container.querySelector(`[id^="erd-definition-list-"]`),
        attributeListContainer: modalElement.querySelector('[data-role="attribute-list-container"]'),
        modalElement,
        modal,
        state: { editingId: null, definitions: [], attributes: [] }
    };

    await refreshErdDefinitions(handlerContext);
    attachErdListeners(handlerContext);
}
