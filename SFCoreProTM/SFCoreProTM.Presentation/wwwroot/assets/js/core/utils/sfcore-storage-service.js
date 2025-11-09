class SFCoreStorageService {
    constructor() {
        this.storage = window.localStorage;
        this.prefix = 'sf_'; // Optional prefix untuk avoid key conflicts
    }

    /**
     * Set data ke LocalStorage dengan auto JSON stringify
     * @param {string} key - Key untuk menyimpan data
     * @param {*} value - Value yang akan disimpan (bisa object, array, dll)
     * @returns {boolean} - Success atau tidak
     */
    set(key, value) {
        try {
            const serializedValue = JSON.stringify(value);
            const storageKey = this.prefix + key;
            this.storage.setItem(storageKey, serializedValue);
            return true;
        } catch (error) {
            console.error('SFCoreStorageService - Error saving to localStorage:', error);
            return false;
        }
    }

    /**
     * Get data dari LocalStorage dengan auto JSON parse
     * @param {string} key - Key untuk mengambil data
     * @param {*} defaultValue - Nilai default jika data tidak ditemukan
     * @returns {*} - Parsed data atau defaultValue
     */
    get(key, defaultValue = null) {
        try {
            const storageKey = this.prefix + key;
            const item = this.storage.getItem(storageKey);
            
            if (item === null) {
                return defaultValue;
            }

            return JSON.parse(item);
        } catch (error) {
            console.error('SFCoreStorageService - Error reading from localStorage:', error);
            return defaultValue;
        }
    }

    /**
     * Hapus data dari LocalStorage
     * @param {string} key - Key yang akan dihapus
     * @returns {boolean} - Success atau tidak
     */
    remove(key) {
        try {
            const storageKey = this.prefix + key;
            this.storage.removeItem(storageKey);
            return true;
        } catch (error) {
            console.error('SFCoreStorageService - Error removing from localStorage:', error);
            return false;
        }
    }

    /**
     * Hapus semua data dengan prefix yang sama
     * @returns {boolean} - Success atau tidak
     */
    clear() {
        try {
            // Hanya clear items dengan prefix yang sama
            const keysToRemove = [];
            for (let i = 0; i < this.storage.length; i++) {
                const key = this.storage.key(i);
                if (key.startsWith(this.prefix)) {
                    keysToRemove.push(key);
                }
            }
            
            keysToRemove.forEach(key => this.storage.removeItem(key));
            return true;
        } catch (error) {
            console.error('SFCoreStorageService - Error clearing storage:', error);
            return false;
        }
    }

    /**
     * Cek apakah key exists di storage
     * @param {string} key - Key yang dicek
     * @returns {boolean} - Exists atau tidak
     */
    has(key) {
        const storageKey = this.prefix + key;
        return this.storage.getItem(storageKey) !== null;
    }

    /**
     * Get semua keys yang ada (dengan prefix yang sama)
     * @returns {string[]} - Array of keys
     */
    keys() {
        const keys = [];
        for (let i = 0; i < this.storage.length; i++) {
            const key = this.storage.key(i);
            if (key.startsWith(this.prefix)) {
                // Remove prefix dari key yang dikembalikan
                keys.push(key.substring(this.prefix.length));
            }
        }
        return keys;
    }

    /**
     * Get jumlah items yang disimpan dengan prefix yang sama
     * @returns {number} - Jumlah items
     */
    size() {
        return this.keys().length;
    }

    /**
     * Set custom prefix untuk storage
     * @param {string} prefix - Prefix baru
     */
    setPrefix(prefix) {
        this.prefix = prefix;
    }

    /**
     * Get data dengan tipe tertentu (convenience methods)
     */
    
    /**
     * Get string (auto convert)
     */
    getString(key, defaultValue = '') {
        const value = this.get(key, defaultValue);
        return String(value);
    }

    /**
     * Get number (auto convert)
     */
    getNumber(key, defaultValue = 0) {
        const value = this.get(key, defaultValue);
        return Number(value);
    }

    /**
     * Get boolean (auto convert)
     */
    getBoolean(key, defaultValue = false) {
        const value = this.get(key, defaultValue);
        return Boolean(value);
    }

    /**
     * Get array (pastikan return array)
     */
    getArray(key, defaultValue = []) {
        const value = this.get(key, defaultValue);
        return Array.isArray(value) ? value : defaultValue;
    }

    /**
     * Get object (pastikan return object)
     */
    getObject(key, defaultValue = {}) {
        const value = this.get(key, defaultValue);
        return typeof value === 'object' && value !== null ? value : defaultValue;
    }
}

// Export untuk module system
if (typeof module !== 'undefined' && module.exports) {
    module.exports = SFCoreStorageService;
} else if (typeof define === 'function' && define.amd) {
    define([], function() { return SFCoreStorageService; });
} else {
    window.SFCoreStorageService = SFCoreStorageService;
}

// Initialize
const sfcoreStorage = new SFCoreStorageService();
