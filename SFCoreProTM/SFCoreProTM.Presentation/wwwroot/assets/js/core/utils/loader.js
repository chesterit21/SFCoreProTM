const loader = document.getElementById('global-loader');
let activeRequests = 0;
let timer;

function show() {
    if (!loader) return;
    loader.classList.add('active');
    loader.style.width = '0%';

    // Animate to a certain percentage to give a sense of progress
    setTimeout(() => {
        if (activeRequests > 0) { // Only animate if still active
            loader.style.width = `${20 + Math.random() * 50}%`; // Random progress
        }
    }, 100);
}

function hide() {
    if (!loader) return;
    loader.style.width = '100%';

    // After completion animation, hide and reset
    setTimeout(() => {
        loader.classList.remove('active');
        loader.style.width = '0%';
    }, 300);
}

export function start() {
    if (activeRequests === 0) {
        show();
    }
    activeRequests++;
    // Clear any existing hide timer
    clearTimeout(timer);
}

export function stop() {
    activeRequests = Math.max(0, activeRequests - 1);

    if (activeRequests === 0) {
        // Use a small delay to handle rapid, subsequent requests gracefully
        timer = setTimeout(hide, 250);
    }
}
