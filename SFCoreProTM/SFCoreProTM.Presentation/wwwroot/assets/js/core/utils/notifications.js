
export function showNotification(message, type = 'info') {
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
}
