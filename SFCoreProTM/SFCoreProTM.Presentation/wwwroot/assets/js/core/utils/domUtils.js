
export function truncateText(text, maxLength) {
    if (!text || text.length <= maxLength) {
        return text;
    }
    return text.substring(0, maxLength) + '...';
}

export function escapeHtml(value) {
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

export function cleanupModal(modalId) {
    const modalElement = document.getElementById(modalId);
    if (modalElement) {
        const modalBody = modalElement.querySelector('.modal-body');
        if (modalBody) {
            modalBody.innerHTML = '';
        }
    }
}
