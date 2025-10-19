// Fungsi untuk memuat script secara dinamis
function loadScript(src) {
    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = src;
        script.onload = () => resolve(window.EditorJS);
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Fungsi untuk memuat plugin Editor.js
function loadPlugin(src) {
    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = src;
        script.onload = () => resolve();
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Fungsi untuk menginisialisasi editor dengan konfigurasi standar
export function initializeEditor(holderId, data = null) {
    // Cek apakah EditorJS sudah dimuat
    if (typeof EditorJS === 'undefined') {
        console.error('EditorJS is not loaded');
        return null;
    }
    
    const defaultData = data || {
        blocks: [
            {
                type: "paragraph",
                data: {
                    text: ""
                }
            }
        ]
    };
    
    const editor = new EditorJS({
        holder: holderId,
        tools: {
            header: {
                class: Header, // Header dari CDN
                inlineToolbar: true
            },
            list: {
                class: List, // List dari CDN
                inlineToolbar: true
            },
            paragraph: {
                class: Paragraph, // Paragraph dari CDN
                inlineToolbar: true
            }
        },
        data: defaultData
    });
    
    return editor;
}

// Fungsi untuk mendapatkan konten teks dari editor
export async function getEditorPlainText(editor) {
    if (!editor) return '';
    
    const editorData = await editor.save();
    return editorData.blocks.map(block => block.data.text).join(' ');
}

// Fungsi untuk mengatur konten editor dari teks
export function setEditorFromPlainText(editor, text) {
    if (!editor) return;
    
    const data = {
        blocks: [
            {
                type: "paragraph",
                data: {
                    text: text || ""
                }
            }
        ]
    };
    
    editor.render(data);
}