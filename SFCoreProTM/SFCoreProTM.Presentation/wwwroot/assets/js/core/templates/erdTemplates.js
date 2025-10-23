
import { escapeHtml } from '../utils/domUtils.js';

function createAttributeInputForm(context) {
    const { moduleId } = context;
    const formId = `attribute-form-${moduleId}`;
    return `
        <div class="card shadow-sm mb-3">
            <div class="card-body">
                <h6 class="card-title mb-3">Add New Attribute</h6>
                <form id="${formId}" class="row g-3 align-items-start">
                    <div class="col-md-4">
                        <label class="form-label">Attribute Name</label>
                        <input type="text" class="form-control" data-field="name" required>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Data Type</label>
                        <select class="form-select" data-field="dataType">
                            <option value="uuid-Guid">uuid (guid)</option>
                            <option value="text-string">text</option>
                            <option value="varchar-string">varchar</option>
                            <option value="char-string">char</option>
                            <option value="integer-int">integer</option>
                            <option value="bigint-long">bigint</option>
                            <option value="smallint-short">smallint</option>
                            <option value="numeric-decimal">numeric</option>
                            <option value="decimal-decimal">decimal</option>
                            <option value="real-float">real</option>
                            <option value="double precision-double">double precision</option>
                            <option value="boolean-bool">boolean</option>
                            <option value="timestamp-DateTime">timestamp</option>
                            <option value="timestamptz-DateTimeOffset">timestamptz</option>
                            <option value="date-DateTime">date</option>
                            <option value="time-TimeSpan">time</option>
                            <option value="interval-TimeSpan">interval</option>
                            <option value="json-string">json</option>
                            <option value="jsonb-string">jsonb</option>
                            <option value="bytea-byte[]">bytea</option>
                            <option value="inet-System.Net.IPAddress">inet</option>
                            <option value="macaddr-System.Net.NetworkInformation.PhysicalAddress">macaddr</option>
                            <option value="serial-int">serial</option>
                            <option value="bigserial-long">bigserial</option>
                        </select>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Max Length</label>
                        <input type="number" class="form-control" data-field="maxChar">
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Description</label>
                        <input type="text" class="form-control" data-field="description">
                    </div>
                    <div class="col-md-6">
                        <div class="row pt-2">
                            <div class="col-12">
                                <label class="form-label">Flags</label>
                                <div class="d-flex flex-wrap gap-3 pt-2">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" data-field="isPrimary">
                                        <label class="form-check-label">Primary Key</label>
                                    </div>
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" data-field="isNull">
                                        <label class="form-check-label">Allow Null</label>
                                    </div>
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" data-field="isForeignKey">
                                        <label class="form-check-label">Foreign Key</label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 mt-3">
                                <label class="form-label">Foreign Key Table</label>
                                <input type="text" class="form-control" data-field="foreignKeyTable">
                            </div>
                        </div>
                    </div>
                    <div class="col-12 text-end">
                        <button type="button" class="btn btn-outline-success-sm" data-action="add-attribute-to-list" title="Add to List">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                </form>
            </div>
        </div>
    `;
}

export function renderAttributeTable(container, attributes, context) {
    const tableBody = attributes.map((attr, index) => {
        const attrId = attr.id ?? attr.Id ?? `temp-${index}`;
        const isPrimary = attr.isPrimary ? '<i class="fas fa-check text-success"></i>' : '';
        const isNull = attr.isNull ? '<i class="fas fa-check text-success"></i>' : '';
        const isForeignKey = attr.isForeignKey ? '<i class="fas fa-check text-success"></i>' : '';
        return `
            <tr draggable="true" data-attribute-id="${attrId}" data-sort-order="${attr.sortOrder}">
                <td>${index + 1}</td>
                <td>${escapeHtml(attr.name)}</td>
                <td>${escapeHtml(attr.dataType)}</td>
                <td>${attr.maxChar || ''}</td>
                <td class="text-center">${isPrimary}</td>
                <td class="text-center">${isNull}</td>
                <td class="text-center">${isForeignKey}</td>
                <td>${escapeHtml(attr.foreignKeyTable || '')}</td>
                <td>${escapeHtml(attr.description || '')}</td>
                <td>
                    <button class="btn btn-outline-danger-sm" data-action="delete-attribute" data-attribute-id="${attrId}" title="Delete Attribute">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    }).join('');

    container.innerHTML = `
        <div class="table-responsive">
            <table class="table table-striped table-hover table-sm" id="attribute-list-table">
                <thead>
                    <tr>
                        <th>Sort</th>
                        <th>Name</th>
                        <th>Data Type</th>
                        <th>Max Length</th>
                        <th>Primary</th>
                        <th>Null</th>
                        <th>Foreign</th>
                        <th>FK Table</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${tableBody}
                </tbody>
            </table>
        </div>
        ${attributes.length === 0 ? '<p class="text-center text-muted">No attributes added yet.</p>' : ''}
    `;
}


export function renderErdModal(container, context) {
    const { moduleId, isErdTask } = context;
    const erdFormId = `erd-definition-form-${moduleId}`;
    const modalId = `erd-modal-${moduleId}`;

    const disabled = isErdTask ? '' : 'disabled';

    const modalHtml = `
        <div class="modal fade" id="${modalId}" tabindex="-1" aria-labelledby="${modalId}Label" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="${modalId}Label">ERD Definition</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <form id="${erdFormId}" class="row g-3">
                            <div class="col-12">
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <label class="form-label">Table Name</label>
                                        <input type="text" class="form-control" name="tName" required ${disabled}>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Entity Name</label>
                                        <input type="text" class="form-control" name="entityName" ${disabled}>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Table Description</label>
                                        <textarea class="form-control" name="description" rows="2" placeholder="Describe this entity" ${disabled}></textarea>
                                    </div>
                                </div>
                            </div>
                        </form>
                        <hr/>
                        <div id="attribute-section-${moduleId}">
                            ${createAttributeInputForm(context)}
                            <h6 class="mt-4 mb-3">Attribute List</h6>
                            <div data-role="attribute-list-container"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-secondary-sm" data-bs-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-outline-success-sm" data-role="erd-submit" ${disabled} title="Save ERD Definition">
                            <i class="fas fa-save"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', modalHtml);
}


export function renderErdSectionLayout(container, context) {
    const { moduleId, isErdTask } = context;
    const erdListId = `erd-definition-list-${moduleId}`;

    const layoutHtml = `
        ${!isErdTask ? `
            <div class="alert alert-warning mb-3" role="alert">
                This task is currently marked as Non-ERD. Editing is disabled.
            </div>
        ` : ''}
        <div class="d-flex justify-content-end mb-3">
             <button class="btn btn-outline-primary-sm" data-action="add-erd" ${!isErdTask ? 'disabled' : ''} title="Add ERD Definition">
                <i class="fas fa-plus"></i>
            </button>
        </div>
        <div id="${erdListId}"></div>
    `;
    container.innerHTML = layoutHtml;
    renderErdModal(container, context);
}


export function renderErdDefinitionsTable(listContainer, definitions, context) {
    if (!listContainer) return;

    if (!definitions || definitions.length === 0) {
        listContainer.innerHTML = '<div class="alert alert-info mb-0">No ERD definitions found for this module.</div>';
        return;
    }

    const tableRows = definitions.map(def => {
        const defId = def.id ?? def.Id;
        return `
            <tr>
                <td>${escapeHtml(def.tName || 'Untitled')}</td>
                <td>${escapeHtml(def.entityName || '')}</td>
                <td>${(def.attributes || []).length}</td>
                <td>${escapeHtml(def.description || 'No description')}</td>
                <td>
                    <div class="btn-group btn-group-sm flex-shrink-0">
                        <button class="btn btn-outline-primary-sm" data-action="edit-erd" data-definition-id="${defId}" ${!context.isErdTask ? 'disabled' : ''} title="Edit">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-outline-danger-sm" data-action="delete-erd" data-definition-id="${defId}" ${!context.isErdTask ? 'disabled' : ''} title="Delete">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');

    listContainer.innerHTML = `
        <div class="table-responsive">
            <table class="table table-striped table-hover">
                <thead>
                    <tr>
                        <th>Table Name</th>
                        <th>Entity Name</th>
                        <th>Attributes</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${tableRows}
                </tbody>
            </table>
        </div>
    `;
}
