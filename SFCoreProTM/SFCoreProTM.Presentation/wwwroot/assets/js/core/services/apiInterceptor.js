import { start, stop } from '../utils/loader.js';

const originalFetch = window.fetch;

window.fetch = function (...args) {
    start();
    const promise = originalFetch.apply(this, args);
    promise.finally(() => {
        stop();
    });
    return promise;
};
