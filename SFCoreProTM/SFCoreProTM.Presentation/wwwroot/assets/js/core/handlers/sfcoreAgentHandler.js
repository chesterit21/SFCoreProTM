// import { showNotification } from '../utils/notifications.js';

document.addEventListener('DOMContentLoaded', () => {
    initAgentTasks();
});

async function initAgentTasks() {

    const userInput = sfcoreStorage.get("inisiateTaskUser");
    //1. Set Chat mode = Agent
    // const chatModeSelect = document.getElementById('chatMode');
    // if (chatModeSelect) {
    //     chatModeSelect.value = "agent";
    //     chatModeSelect.dispatchEvent(new Event('change'));
    // }

    //2. Clear chat messages and open chat
    setTimeout(() => {
        document.getElementById('chatMessages').innerHTML = '';
        const btnOpenChat = document.getElementById('sfcore-ai-chat-btn');
        btnOpenChat?.click(); // Optional chaining untuk safety
    }, 500); // Delay 900ms

    //3. Display thinking message
    const followUpMessages = generateThinkDisplay();
    const thinkMapper = `<think>${followUpMessages}</think>`;
    setTimeout(() => {
        ChatTextAppender.appendTextToChat(thinkMapper, {
            thinkingMode: true,
            streaming: true,
            streamDelay: 45
        });
    }, 5000);

    console.log('mulai mengirim pesan ke server');
    const btnSendChat = document.getElementById('sendChatMessage');
    console.log('Button status:', btnSendChat); // Cek apakah null/undefined

    if (btnSendChat) {
        btnOpenChat.click();
        console.log('Auto-click triggered!');
    }
    else {
        ChatHandler.sendMessage();d
    }
    //2. set send message

    // Close existing connection if any
    // if (this.eventSource) {
    //     this.eventSource.close();
    //     this.eventSource = null;
    // }

    // const initSse = async (prompt) => {
    //     // SSE Auto with EventSource (optimized: GET query params, no model param)
    //     const url = new URL('/api/agentba/sse', "http://localhost:5282");
    //     url.searchParams.append('prompt', encodeURIComponent(prompt)); // Security: encode
    //     this.eventSource = new EventSource(url.toString());
    //     this.eventSource.onopen = () => {
    //         console.log('SSE connected');
    //         this.currentBuffer = ''; // Reset buffer
    //         this.fullContentBuffer = ''; // Reset full content buffer
    //         this.reconnectAttempts = 0; // Reset reconnect attempts on successful connection
    //     };

    //     this.eventSource.onmessage = (event) => {
    //         const data = event.data.trim();
    //         if (data === '[DONE]') {
    //             this.eventSource.close();
    //             this.eventSource = null;
    //             return;
    //         }

    //         try {
    //             const parsed = JSON.parse(data);
    //             if (parsed.type === 'token') {
    //                 const decodedData = parsed.data.replace(/%20/g, ' ');
    //                 ChatTextAppender.appendMultipleMessages(decodedData, {
    //                     streaming: true,
    //                     delayBetweenMessages: 20
    //                 });                // Debounced append (performance: reduce DOM writes)
    //             } else if (parsed.type === 'error') {
    //                 this.eventSource.close();
    //                 this.eventSource = null;
    //             }
    //         } catch (parseErr) {
    //             console.warn('SSE parse error:', parseErr, data);
    //             this.currentBuffer += `[Parse: ${data}]`;
    //             this.fullContentBuffer += `[Parse: ${data}]`;
    //             this.debouncedAppend(aiMsg, `[Parse Error]`);
    //         }
    //     };

    //     this.eventSource.onerror = (err) => {
    //         console.error('SSE error:', err);

    //         // Increment reconnect attempts
    //         this.reconnectAttempts++;

    //         // Only show error message if not already shown and within max attempts
    //         if (!this.errorShown && this.reconnectAttempts <= this.maxReconnectAttempts) {
    //             this.errorShown = true;
    //         }

    //         // Auto-reconnect only if within max attempts
    //         if (this.reconnectAttempts <= this.maxReconnectAttempts) {
    //             setTimeout(() => {
    //                 if (this.eventSource?.readyState === EventSource.CLOSED) {
    //                     console.log(`Attempting reconnect ${this.reconnectAttempts}/${this.maxReconnectAttempts}`);
    //                     this.handleChatMode(prompt); // Retry dengan prompt yang sama
    //                 }
    //             }, 3000);
    //         } else {
    //             // Max attempts reached, show final error and close connection
    //             if (this.reconnectAttempts === this.maxReconnectAttempts + 1) {
    //                 this.addErrorMessage('Koneksi gagal setelah beberapa percobaan. Silakan coba lagi nanti.');
    //                 this.eventSource.close();
    //                 this.eventSource = null;
    //             }
    //         }
    //     };

    // };
    // initSse(userInput);
    // Auto-handlers (real-world: robust error/reconnect)

    // try {
    //     const url = new URL('http://localhost:5282/api/agentba/handleProject');
    //     url.searchParams.append('prompt', encodeURIComponent(userInput));
    //     const response = await fetch(url.toString());
    //     if (!response.ok) {
    //         throw new Error(`HTTP error! status: ${response.status}`);
    //     }
    //     console.log('Response from server:');
    //     console.log(response);
    //     // const data = await response.json();
    //     // console.log('data from server:');
    //     // console.log(data);
    //     // // Process your data here instead of in onmessage
    //     // this.appendFinalBuffer(aiMsg); // Uncomment if needed

    // } catch (error) {
    //     console.error('Fetch error:', error);
    // }
}

// Generate follow-up messages berdasarkan tipe tugas
function generateThinkDisplay() {
    const messages = [];

    // Always start with analysis phase
    messages.push('Pak Bos memberikan tugas kepada saya, dan saya harus benar-benar dalam melaksakan tugas');
    messages.push('saya akan memulai analisis mendalam terhadap requirements.\n\nSaya harap Pak Bos Mau menunggu');
    messages.push('karena saya harus berpikir mendalam terhadap tugas yang diberikan. Juga saya adalah SFCore Agent AI dengan 30B parameter, perlu waktu berpikir lama karena saya memiliki kecerdasan yang tinggi dan TINGKAT DEWA 😎 serta SUPER DEWA 😊 sebelum memberikan jawaban yang tepat dan sesuai konteks nya.');
    messages.push('\n\nTidak diragukan lagi kemampuan saya dibandingkan dengan AI lainnya, bahkan ChatGPT dan Gemini secara official mengakui kemampuan SFCore Agent AI yang sudah di training Pak Bos.');
    messages.push('Saya tidak bermaksud menyombongkan diri atau ujub , tetapi itulah kenyataan nya, semua DUNIA sudah mengakui nya 😎 😊, sertifikasi saya sudah banyak, client saya merasa puas juga semua nya 😊😎');
    messages.push(' Tanpa berlama-lama lagi, saya akan memulai analisis mendalam terhadap requirements. Mohon bersabar ya Pak Bos....😊🔥, saya akan mulai bernalar ayo semangat 💪');
    return messages;
}