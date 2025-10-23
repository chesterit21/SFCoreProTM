
import { cleanupModal } from '../utils/domUtils.js';
import { showNotification } from '../utils/notifications.js';

/**
 * Fetches HTML content from a URL and loads it into a modal.
 * @param {string} modalId The ID of the modal element.
 * @param {string} url The URL to fetch the modal content from.
 * @param {function(HTMLElement): void} [setupCallback] Optional callback to run after modal content is loaded. Receives the modal element.
 */
export async function openModal(modalId, url, setupCallback) {
    const modalElement = document.getElementById(modalId);
    if (!modalElement) {
        console.error(`Modal element with ID '${modalId}' not found.`);
        showNotification('UI component is missing.', 'danger');
        return;
    }

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const html = await response.text();

        const modalBody = modalElement.querySelector('.modal-body');
        if (modalBody) {
            modalBody.innerHTML = html;
        }

        // Run the setup callback to attach event listeners, etc.
        if (typeof setupCallback === 'function') {
            setupCallback(modalElement);
        }

        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();

        // Ensure cleanup happens when modal is closed
        modalElement.addEventListener('hidden.bs.modal', () => {
            cleanupModal(modalId);
        }, { once: true });

    } catch (error) {
        console.error(`Error loading modal content from ${url}:`, error);
        showNotification(`Error loading content: ${error.message}`, 'danger');
    }
}
