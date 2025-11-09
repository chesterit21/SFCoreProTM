// Fungsi untuk append text ke chat dari variable string (standalone)
const ChatTextAppender = {
    /**
     * Append text ke AI chat message dengan efek streaming atau langsung
     * @param {string} text - Text yang akan ditampilkan
     * @param {Object} options - Opsi tambahan
     * @param {boolean} options.streaming - Efek streaming karakter per karakter
     * @param {number} options.streamDelay - Delay per karakter (ms)
     * @param {boolean} options.thinkingMode - Mode thinking dengan expand/collapse
     */
    appendTextToChat(text, options = {}) {
        const {
            streaming = false,
            streamDelay = 20,
            thinkingMode = false
        } = options;

        const chatMessages = document.getElementById('chatMessages');
        if (!chatMessages) {
            console.error('Chat messages container not found');
            return;
        }

        // Create AI message element
        const aiMsg = this.createMessageElement('', 'ai');
        aiMsg.style.whiteSpace = 'pre-wrap';
        chatMessages.appendChild(aiMsg);
        this.scrollToBottom(chatMessages);

        // Handle thinking mode jika diperlukan
        if (thinkingMode && (text.includes('<think>') || text.includes('<thinking>'))) {
            text = text.replace(/<think>/g, '').replace(/<\/think>/g, '');
            this.createThinkingContainer(aiMsg);
            const thinkingContainer = aiMsg.querySelector('.thinking-container');
            if (thinkingContainer) {
                const thinkingText = thinkingContainer.querySelector('.thinking-text');
                if (thinkingText) {
                    if (streaming) {
                        this.streamText(thinkingText, text, streamDelay);
                    } else {
                        thinkingText.innerHTML = this.sanitizeHtml(text);
                    }
                }
            }
        } else {
            // Regular message
            if (streaming) {
                this.streamText(aiMsg, text, streamDelay);
            } else {
                aiMsg.innerHTML = this.sanitizeHtml(text);
            }
        }

        this.scrollToBottom(chatMessages);
    },

    /**
     * Stream text dengan efek ketik per karakter
     */
    streamText(container, text, delay = 20) {
        return new Promise((resolve) => {
            let index = 0;
            const sanitizedText = this.sanitizeHtml(text);
            
            container.innerHTML = '';
            
            const interval = setInterval(() => {
                if (index < sanitizedText.length) {
                    // Handle HTML tags properly during streaming
                    if (sanitizedText.substring(index, index + 4) === '&lt;') {
                        // Find the closing tag
                        const closingIndex = sanitizedText.indexOf('&gt;', index);
                        if (closingIndex !== -1) {
                            container.innerHTML = sanitizedText.substring(0, closingIndex + 4);
                            index = closingIndex + 4;
                        } else {
                            container.innerHTML = sanitizedText.substring(0, index + 1);
                            index++;
                        }
                    } else {
                        container.innerHTML = sanitizedText.substring(0, index + 1);
                        index++;
                    }
                    
                    this.scrollToBottom(document.getElementById('chatMessages'));
                } else {
                    clearInterval(interval);
                    resolve();
                }
            }, delay);
        });
    },

    /**
     * Create message element (mirip dengan utils.createMessageElement)
     */
    createMessageElement(text, sender) {
        const el = document.createElement('div');
        el.className = `chat-message ${sender}`;
        el.setAttribute('role', 'log');
        el.setAttribute('aria-live', sender === 'ai' ? 'polite' : 'off');
        el.innerHTML = text ? this.sanitizeHtml(text) : '';
        return el;
    },

    /**
     * Sanitize HTML (mirip dengan utils.sanitizeHtml)
     */
    sanitizeHtml(str) {
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML.replace(/\n/g, '</br>');
    },

    /**
     * Scroll to bottom (mirip dengan utils.scrollToBottom)
     */
    scrollToBottom(container) {
        if (container) {
            container.scrollTo({
                top: container.scrollHeight,
                behavior: 'smooth'
            });
        }
    },

    /**
     * Create thinking container (mirip dengan ChatHandler.createThinkingContainer)
     */
    createThinkingContainer(container) {
        if (container.querySelector('.thinking-container')) {
            return;
        }
        
        const thinkingContainer = document.createElement('div');
        thinkingContainer.className = 'thinking-container';
        thinkingContainer.innerHTML = `
            <div class="thinking-header" style="cursor: pointer; background: #ffffffff;" 
                 role="button" tabindex="0" aria-expanded="true" aria-label="Toggle thinking details">
                <span class="toggle-icon" aria-hidden="true">▼</span> Thinking...
            </div>
            <div class="thinking-content" style="display: block;">
                <div class="thinking-text" style="white-space: pre-wrap;"></div>
            </div>
        `;

        const header = thinkingContainer.querySelector('.thinking-header');
        const contentDiv = thinkingContainer.querySelector('.thinking-content');
        const toggleIcon = thinkingContainer.querySelector('.toggle-icon');

        const toggle = () => {
            const isExpanded = contentDiv.style.display !== 'none';
            contentDiv.style.display = isExpanded ? 'none' : 'block';
            toggleIcon.textContent = isExpanded ? '►' : '▼';
            header.setAttribute('aria-expanded', (!isExpanded).toString());
        };

        header.addEventListener('click', toggle);
        header.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggle();
            }
        });

        container.appendChild(thinkingContainer);
    },

    /**
     * Append multiple messages sekaligus
     */
    appendMultipleMessages(messages, options = {}) {
        messages.forEach((message, index) => {
            setTimeout(() => {
                this.appendTextToChat(message, options);
            }, index * (options.delayBetweenMessages || 1000));
        });
    }
};

// Export untuk global access
window.ChatTextAppender = ChatTextAppender;


// // 1. Append text biasa (langsung muncul)
// ChatTextAppender.appendTextToChat('Halo bro! Ini pesan langsung dari variable.');

// // 2. Append dengan efek streaming (karakter per karakter)
// ChatTextAppender.appendTextToChat('Ini pesan dengan efek ketik seperti AI beneran...', {
//     streaming: true,
//     streamDelay: 30 // ms per karakter
// });

// // 3. Append dengan thinking mode
// ChatTextAppender.appendTextToChat('<thinking>Saya sedang menganalisa permintaan user...</thinking>', {
//     thinkingMode: true,
//     streaming: true
// });

// // 4. Append multiple messages
// ChatTextAppender.appendMultipleMessages([
//     'Pertama, saya akan setup project structure...',
//     'Kedua, configure dependencies...',
//     'Terakhir, implement business logic...'
// ], {
//     streaming: true,
//     delayBetweenMessages: 1500
// });

// // 5. Kombinasi thinking + regular
// ChatTextAppender.appendTextToChat('<thinking>Analisis requirement: user butuh fitur CRUD...</thinking>', {
//     thinkingMode: true
// });

// setTimeout(() => {
//     ChatTextAppender.appendTextToChat('Oke, saya sudah selesai menganalisa. Mari kita mulai implementasinya!', {
//         streaming: true
//     });
// }, 2000);
