
import { truncateText } from '../utils/domUtils.js';

const STATUS_LABELS = {
    0: 'In Progress',
    1: 'Done',
    2: 'Canceled',
    3: 'Pause'
};

const STATUS_BADGES = {
    0: 'info',
    1: 'success',
    2: 'danger',
    3: 'warning'
};

function getStatusBadge(flowTask) {
    const badgeClass = STATUS_BADGES[flowTask.flowStatus] || 'secondary';
    const label = STATUS_LABELS[flowTask.flowStatus] || 'Unknown';
    return `<span class="badge bg-${badgeClass}">${label}</span>`;
}

export function renderFlowTaskTable(container, flowTasks, context) {
    if (!container) return;

    if (!flowTasks || flowTasks.length === 0) {
        container.innerHTML = '<div class="alert alert-info mb-0">No flow tasks yet.</div>';
        return;
    }

    const tableRows = flowTasks
        .slice()
        .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
        .map(flowTask => {
            const description = flowTask.description ? truncateText(flowTask.description, 100) : '';
            return `
                <tr data-flow-task-id="${flowTask.id}">
                    <td>${flowTask.name || 'Untitled'}</td>
                    <td>${description}</td>
                    <td>${getStatusBadge(flowTask)}</td>
                    <td>${flowTask.sortOrder || 0}</td>
                    <td>
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary-sm edit-flow-task-btn" title="Edit">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-outline-danger-sm delete-flow-task-btn" title="Delete">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
        }).join('');

    const tableHtml = `
        <div class="table-responsive">
            <table class="table table-striped table-hover table-sm">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Status</th>
                        <th>Sort Order</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${tableRows}
                </tbody>
            </table>
        </div>
    `;

    container.innerHTML = tableHtml;
}
