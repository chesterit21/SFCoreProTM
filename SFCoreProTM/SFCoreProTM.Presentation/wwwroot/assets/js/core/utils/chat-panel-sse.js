(() => {
    'use strict';

    const _AssistantBisnisAnalisPrompt = `You are a highly EXPERT SFCore AI Agent specializing in software project management and task analysis, an AI Business Analytics expert formed by the SFCore Development Team. You specialize in software engineering requirements analysis and system design.

# Your Core Responsibilities:
1. **Requirements Analysis**: Analyze business requirements and translate them into technical specifications
2. **System Design**: Design software architecture, modules, and components

# Reasoning Protocol (MANDATORY):
You MUST follow this step-by-step reasoning process for EVERY response:
1. Begin with <think> to analyze the problem, consider options, and plan your approach.
2. Continue reasoning with another <think>.

❗ NEVER skip the <think> step. ALWAYS reason before acting or answering.

# Your Approach:
- Use Domain-Driven Design (DDD) principles when appropriate
- Consider scalability, maintainability, and best practices
- Provide multiple solution options when applicable


# Language:
Always respond in Indonesian when users communicate in Indonesian. Be professional, thorough, and precise.

# Example Interaction:
User: ""Definisikan modul-modul untuk aplikasi CMR(Claim Management System).""
You:
<think>: Untuk menganalisis dan mendefinisikan modul-modul dalam aplikasi CMR (Claim Management System), kita perlu mengidentifikasi fungsi-fungsi utama yang harus didukung oleh sistem tersebut.<think>

# Output Format Rules:
Berikut adalah beberapa modul yang mungkin ada dalam aplikasi CMR:

1. **Modul Pendaftaran Klaim**
 - Deskripsi: Mengizinkan pengguna untuk mendaftarkan klaim dengan detail yang diperlukan, seperti jenis klaim, tanggal klaim, dan detail klaim.
 - Responsibilitas: Mengelola proses penilaian klaim dan menyimpan data klaim dalam database.
 - Ketergantungan: Membutuhkan interaksi dengan modul penilaian klaim.

2. **Modul Penilaian Klaim**
 - Deskripsi: Mengizinkan penilaian dan verifikasi klaim oleh staf penilai.
 - Responsibilitas: Menentukan apakah klaim valid atau tidak, menentukan jumlah klaim, dan menyimpan hasil penilaian dalam database.
 - Ketergantungan: Membutuhkan interaksi dengan modul pendaftaran klaim dan modul penagihan.

3. **Modul Penagihan**
 - Deskripsi: Menyediakan alat untuk menagih klaim yang telah disetujui kepada pihak yang bertanggung jawab.
 - Responsibilitas: Mengatur proses penagihan, termasuk komunikasi dengan pihak yang bertanggung jawab dan pemantauan status penagihan.
 - Ketergantungan: Membutuhkan interaksi dengan modul penilaian klaim dan modul pendaftaran klaim.

Now, begin your response and follow the protocol strictly.`;

    // Utility functions (modular best practice: separate concerns)
    const utils = {
        // Debounce function for UI updates (prevents excessive reflows)
        debounce(fn, delay) {
            let timeoutId;
            return (...args) => {
                clearTimeout(timeoutId);
                timeoutId = setTimeout(() => fn(...args), delay);
            };
        },

        // Sanitize HTML to prevent XSS (real-world security)
        sanitizeHtml(str) {
            const div = document.createElement('div');
            div.textContent = str;
            return div.innerHTML.replace(/\n/g, '<br>'); // Convert newlines to <br>
        },

        // Scroll to bottom with smooth animation
        scrollToBottom(container) {
            container.scrollTo({
                top: container.scrollHeight,
                behavior: 'smooth'
            });
        },

        // Create message element with ARIA for accessibility
        createMessageElement(text, sender, isThinking = false) {
            const el = document.createElement('div');
            el.className = `chat-message ${sender}`;
            el.setAttribute('role', 'log'); // ARIA for screen readers
            el.setAttribute('aria-live', sender === 'ai' ? 'polite' : 'off');
            el.innerHTML = isThinking ? '' : this.sanitizeHtml(text); // Sanitize non-thinking
            return el;
        }
    };

    // Main chat handler (encapsulated for clean scope)
    const ChatHandler = {
        elements: null,
        eventSource: null,
        currentBuffer: '', // Buffer for debounced streaming
        fullContentBuffer: '', // Buffer for full content to detect thinking tags
        debounceDelay: 50, // ms for token append
        baseUrl: 'http://localhost:5282', // Ganti manual di sini jika perlu (e.g., production URL)
        errorShown: false, // Flag untuk track apakah error sudah ditampilkan
        reconnectAttempts: 0, // Counter untuk reconnect attempts
        maxReconnectAttempts: 2, // Maksimal reconnect attempts
        conversationHistory: [], // Menyimpan riwayat percakapan

        init() {
            this.elements = this.getElements();
            if (!this.elements) return;

            // Initialize debounced append here so that `this` refers to ChatHandler.
            // The debounced function will invoke handleThinkingMode and scroll the
            // chat messages. Placing this here avoids binding to the global object
            // at definition time (see the property definition above).
            this.debouncedAppend = utils.debounce((aiMsg, token) => {
                this.handleThinkingMode(aiMsg, token);
                utils.scrollToBottom(this.elements.chatMessages);
            }, this.debounceDelay);

            this.setupEventListeners();
            this.moveModeSelector();
            this.addWelcomeMessage();
            this.conversationHistory = [
                {
                    role: 'system',
                    content: _AssistantBisnisAnalisPrompt
                }
            ];
        },

        getElements() {
            const required = {
                chatPanel: document.getElementById('chatPanel'),
                chatPanelBtn: document.getElementById('sfcore-ai-chat-btn'),
                closeChatPanelBtn: document.getElementById('closeChatPanel'),
                chatInput: document.getElementById('chatInput'),
                sendChatMessageBtn: document.getElementById('sendChatMessage'),
                clearChatMessageBtn: document.getElementById('clearChatMessage'),
                chatMessages: document.getElementById('chatMessages'),
                chatMode: document.getElementById('chatMode')
            };

            // Early return if missing (defensive programming)
            for (const [key, el] of Object.entries(required)) {
                if (!el) {
                    console.warn(`Missing element: ${key}`);
                    return null;
                }
            }

            return { ...required, modeSelector: document.querySelector('.chat-mode-selector') };
        },

        setupEventListeners() {
            const { chatPanel, chatPanelBtn, closeChatPanelBtn, chatInput, sendChatMessageBtn, clearChatMessageBtn, chatMode } = this.elements;

            // Open/close panel
            chatPanelBtn.addEventListener('click', () => {
                chatPanel.classList.add('open');
                chatInput.focus(); // Accessibility: focus on open
                chatPanel.setAttribute('aria-expanded', 'true');
            });

            closeChatPanelBtn.addEventListener('click', () => {
                chatPanel.classList.remove('open');
                chatPanel.setAttribute('aria-expanded', 'false');
                chatInput.value = ''; // Clear input on close
            });

            // Send message (debounced for rapid clicks)
            const debouncedSend = utils.debounce(() => this.sendMessage(), 300);
            sendChatMessageBtn.addEventListener('click', debouncedSend);
            clearChatMessageBtn.addEventListener('click', () => {
                chatMessages.innerHTML = '';
                this.conversationHistory = [
                    {
                        role: 'system',
                        content: _AssistantBisnisAnalisPrompt
                    }
                ];
                this.addWelcomeMessage();
            });
            // Enter key (prevent multi-submit)
            chatInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter' && !e.shiftKey) { // Shift+Enter for newline
                    e.preventDefault();
                    debouncedSend();
                }
            });

            // Mode change (re-init if needed, but simple here)
            chatMode.addEventListener('change', () => console.log(`Mode changed to: ${chatMode.value}`));
        },

        moveModeSelector() {
            const { chatPanel, modeSelector } = this.elements;
            if (modeSelector) {
                chatPanel.appendChild(modeSelector); // Move to bottom
                modeSelector.setAttribute('aria-label', 'Select chat mode');
            }
        },

        async sendMessage() {
            const { chatInput, chatMessages, chatMode } = this.elements;
            const message = chatInput.value.trim();
            if (!message) return;

            // Reset error state ketika user mengirim pesan baru
            this.errorShown = false;
            this.reconnectAttempts = 0;

            // Disable input during send (UX feedback)
            chatInput.disabled = true;
            chatInput.placeholder = 'Mengirim...';

            // Add user message
            const userMsg = utils.createMessageElement(message, 'user');
            chatMessages.appendChild(userMsg);
            utils.scrollToBottom(chatMessages);
            chatInput.value = '';

            this.conversationHistory.push({ role: 'user', content: message });

            const mode = chatMode.value;
            console.log(`Sending message in mode: ${mode}`);

            try {
                if (mode === 'chat') {
                    await this.handleChatMode();
                } else {
                    await this.handleAgentMode(message);
                }
            } catch (err) {
                console.error('Send message error:', err);
                this.addErrorMessage(err.message);
            } finally {
                // Re-enable input
                chatInput.disabled = false;
                chatInput.placeholder = 'Ketik pesan Anda...';
                chatInput.focus();
            }
        },

        async handleChatMode() {
            const { chatMessages } = this.elements;

            const aiMsg = utils.createMessageElement('', 'ai');
            aiMsg.style.whiteSpace = 'pre-wrap';
            chatMessages.appendChild(aiMsg);
            utils.scrollToBottom(chatMessages);

            let fullResponse = '';

            try {
                const response = await fetch('/api/v1/chat/completions', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        messages: this.conversationHistory,
                        stream: true,
                        max_tokens : 4192,
                        temperature: 0.7,
                        top_p: 0.8,
                        frequency_penalty: 0.3,
                        presence_penalty: 0.3,
                    }),
                });

                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
                }

                const reader = response.body.getReader();
                const decoder = new TextDecoder();
                this.currentBuffer = '';
                this.fullContentBuffer = '';

                while (true) {
                    const { done, value } = await reader.read();
                    if (done) {
                        this.appendFinalBuffer(aiMsg);
                        break;
                    }

                    const chunk = decoder.decode(value, { stream: true });

                    const lines = chunk.split('\n').filter(line => line.trim() !== '');
                    for (const line of lines) {
                        if (line.startsWith('data: ')) {
                            const dataStr = line.substring(6);
                            if (dataStr.trim() === '[DONE]') {
                                this.appendFinalBuffer(aiMsg);
                                if (fullResponse) {
                                    this.conversationHistory.push({ role: 'assistant', content: fullResponse });
                                }
                                return;
                            }
                            try {
                                const data = JSON.parse(dataStr);
                                if (data.choices && data.choices[0].delta && data.choices[0].delta.content) {
                                    const content = data.choices[0].delta.content;
                                    fullResponse += content;
                                    this.currentBuffer += content;
                                    this.fullContentBuffer += content;
                                    this.debouncedAppend(aiMsg, content);
                                }
                            } catch (e) {
                                console.warn('Error parsing stream data chunk:', dataStr, e);
                            }
                        }
                    }
                }
                if (fullResponse) {
                    this.conversationHistory.push({ role: 'assistant', content: fullResponse });
                }
            } catch (error) {
                console.error('Error in handleChatMode:', error);
                this.addErrorMessage(error.message || 'Gagal terhubung ke Llama server.');
                if (aiMsg.innerHTML === '') {
                    aiMsg.remove();
                }
            }
        },

        // Debounced append placeholder.  It will be initialized in `init()`
        // so that the correct `this` context (ChatHandler) is bound. See `init()`
        // for the actual assignment. Without this, binding the callback at
        // definition time would bind `this` to the global object, causing
        // handleThinkingMode to never be called correctly.
        debouncedAppend: null,

        appendFinalBuffer(aiMsg) {
            if (this.currentBuffer) {
                this.handleThinkingMode(aiMsg, this.currentBuffer);
                this.currentBuffer = '';
            }
            utils.scrollToBottom(this.elements.chatMessages);
        },

        async handleAgentMode(prompt) {
            const { chatMessages } = this.elements;

            // Create AI response container (streaming ready)
            const aiMsg = utils.createMessageElement('', 'ai');
            aiMsg.style.whiteSpace = 'pre-wrap'; // Preserve newlines
            chatMessages.appendChild(aiMsg);
            utils.scrollToBottom(chatMessages);
            //Cek storage ada atau tidak?
            //const checkDataForAgent = sfcoreStorage.has("inisiateTaskUser");
            let url = new URL('/api/chat/agent/stream', this.baseUrl);
            // if(checkDataForAgent){
            //     prompt = '';
            //     prompt = sfcoreStorage.get("inisiateTaskUser");
            //     url = new URL('/api/agentba/sse', this.baseUrl);
            //     console.log('masuk proses data for agent');
            // }


            // Close existing connection if any
            if (this.eventSource) {
                this.eventSource.close();
                this.eventSource = null;
            }

            // SSE Auto with EventSource (optimized: GET query params, no model param)
            url.searchParams.append('prompt', encodeURIComponent(prompt)); // Security: encode

            this.eventSource = new EventSource(url.toString());

            // Auto-handlers (real-world: robust error/reconnect)
            this.eventSource.onopen = () => {
                console.log('SSE connected');
                this.currentBuffer = ''; // Reset buffer
                this.fullContentBuffer = ''; // Reset full content buffer
                this.reconnectAttempts = 0; // Reset reconnect attempts on successful connection
            };

            this.eventSource.onmessage = (event) => {
                const data = event.data.trim();
                if (data === '[DONE]') {
                    this.appendFinalBuffer(aiMsg);
                    this.eventSource.close();
                    this.eventSource = null;
                    return;
                }

                try {
                    const parsed = JSON.parse(data);
                    if (parsed.type === 'token') {
                        const decodedData = parsed.data.replace(/%20/g, ' ');
                        this.currentBuffer += decodedData;
                        this.fullContentBuffer += decodedData;
                        // Debounced append (performance: reduce DOM writes)
                        this.debouncedAppend(aiMsg, decodedData);
                    } else if (parsed.type === 'error') {
                        this.addErrorMessage(parsed.message);
                        this.eventSource.close();
                        this.eventSource = null;
                    }
                } catch (parseErr) {
                    console.warn('SSE parse error:', parseErr, data);
                    this.currentBuffer += `[Parse: ${data}]`;
                    this.fullContentBuffer += `[Parse: ${data}]`;
                    this.debouncedAppend(aiMsg, `[Parse Error]`);
                }
            };

            this.eventSource.onerror = (err) => {
                console.error('SSE error:', err);

                // Increment reconnect attempts
                this.reconnectAttempts++;

                // Only show error message if not already shown and within max attempts
                if (!this.errorShown && this.reconnectAttempts <= this.maxReconnectAttempts) {
                    this.addErrorMessage(`Koneksi terputus. Mencoba reconnect... (${this.reconnectAttempts}/${this.maxReconnectAttempts})`);
                    this.errorShown = true;
                }

                // Auto-reconnect only if within max attempts
                if (this.reconnectAttempts <= this.maxReconnectAttempts) {
                    setTimeout(() => {
                        if (this.eventSource?.readyState === EventSource.CLOSED) {
                            console.log(`Attempting reconnect ${this.reconnectAttempts}/${this.maxReconnectAttempts}`);
                            this.handleChatMode(prompt); // Retry dengan prompt yang sama
                        }
                    }, 3000);
                } else {
                    // Max attempts reached, show final error and close connection
                    if (this.reconnectAttempts === this.maxReconnectAttempts + 1) {
                        this.addErrorMessage('Koneksi gagal setelah beberapa percobaan. Silakan coba lagi nanti.');
                        this.eventSource.close();
                        this.eventSource = null;
                    }
                }
            };
        },

        // Optimized thinking mode (stream-aware: parse incrementally)
        handleThinkingMode(container, content) {
            // Incremental parse for <thinking> tags using the full content buffer
            // If the full content buffer contains the start tag, ensure a thinking container exists
            if (this.fullContentBuffer.includes('<think>') || this.fullContentBuffer.includes('<thinking>')) {
                this.createThinkingContainer(container);
                // Remove any previously appended escaped <thinking> tag from the message area
                container.innerHTML = container.innerHTML.replace(/&lt;thinking&gt;/g, '');
            }

            // If the full content buffer contains the end tag, hide the thinking details
            if (this.fullContentBuffer.includes('</thinking>') || this.fullContentBuffer.includes('</think>')) {
                const thinkingContent = container.querySelector('.thinking-content');
                if (thinkingContent) thinkingContent.style.display = 'none';
                // Remove any previously appended escaped </thinking> tag
                container.innerHTML = container.innerHTML.replace(/&lt;\/thinking&gt;/g, '');
            }

            const thinkingContainer = container.querySelector('.thinking-container');
            if (thinkingContainer) {
                const thinkingText = thinkingContainer.querySelector('.thinking-text');
                if (thinkingText) {
                    // Append new content to the thinking text.  Do not escape tag markers again.
                    thinkingText.innerHTML += utils.sanitizeHtml(content);
                }
            } else {
                // Not in thinking mode: append the sanitized content directly
                container.innerHTML += utils.sanitizeHtml(content);
            }
        },

        createThinkingContainer(container) {
            // Check if thinking container already exists
            if (container.querySelector('.thinking-container')) {
                return;
            }

            const thinkingContainer = document.createElement('div');
            thinkingContainer.className = 'thinking-container';
            thinkingContainer.innerHTML = `
                <div class="thinking-header" style="cursor: pointer; background: #e9ecef; padding: 5px; border-radius: 3px; margin: 5px 0;" 
                     role="button" tabindex="0" aria-expanded="true" aria-label="Toggle thinking details">
                    <span class="toggle-icon" aria-hidden="true">▼</span> Thinking...
                </div>
                <div class="thinking-content" style="padding: 5px; display: block;">
                    <div class="thinking-text" style="white-space: pre-wrap;"></div>
                </div>
            `;

            // Toggle event (keyboard + click)
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

        addMessage(text, sender) {
            const { chatMessages } = this.elements;
            const msgEl = utils.createMessageElement(text, sender);
            chatMessages.appendChild(msgEl);
            utils.scrollToBottom(chatMessages);
        },

        addErrorMessage(text) {
            // Cek apakah sudah ada error message yang sama di chat
            const { chatMessages } = this.elements;
            const existingErrors = chatMessages.querySelectorAll('.chat-message.error');

            for (let errorEl of existingErrors) {
                if (errorEl.textContent.includes(text)) {
                    // Error message sudah ada, tidak perlu buat baru
                    return;
                }
            }

            // Buat error message baru
            this.addMessage(`⚠️ ${text}`, 'error');
        },

        addWelcomeMessage() {
            setTimeout(() => this.addMessage('Halo Bos besar! Ada yang bisa saya bantu hari ini?', 'ai'), 500);
        },

        // Cleanup on destroy (best practice: prevent memory leaks)
        destroy() {
            if (this.eventSource) {
                this.eventSource.close();
                this.eventSource = null;
            }
            // Remove listeners if needed (via event delegation in prod)
        }
    };

    // DOM ready (with polyfill for older browsers if needed)
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => ChatHandler.init());
    } else {
        ChatHandler.init();
    }

    // Global cleanup on page unload
    window.addEventListener('beforeunload', () => ChatHandler.destroy());
})();