(() => {
    // Wait for DOM to be loaded
    document.addEventListener('DOMContentLoaded', () => {
        // Get DOM elements
        const chatPanel = document.getElementById('chatPanel');
        const chatPanelBtn = document.getElementById('sfcore-ai-chat-btn');
        const closeChatPanelBtn = document.getElementById('closeChatPanel');
        const chatInput = document.getElementById('chatInput');
        const sendChatMessageBtn = document.getElementById('sendChatMessage');
        const chatMessages = document.getElementById('chatMessages');
        const chatMode = document.getElementById('chatMode');

        // Check if all elements exist
        if (!chatPanel || !chatPanelBtn || !closeChatPanelBtn || !chatInput ||
            !sendChatMessageBtn || !chatMessages || !chatMode) {
            console.warn('Some chat panel elements are missing');
            return;
        }

        // Get mode selector element
        const modeSelector = document.querySelector('.chat-mode-selector');

        // Move mode selector to bottom of chat panel
        if (modeSelector) {
            chatPanel.appendChild(modeSelector);
        }

        // Open chat panel
        chatPanelBtn.addEventListener('click', () => {
            chatPanel.classList.add('open');
            chatInput.focus();
        });

        // Close chat panel
        closeChatPanelBtn.addEventListener('click', () => {
            chatPanel.classList.remove('open');
        });

        // Send message when button is clicked
        sendChatMessageBtn.addEventListener('click', sendMessage);

        // Send message when Enter is pressed
        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                sendMessage();
            }
        });

        // Function to send a message
        async function sendMessage() {
            const message = chatInput.value.trim();
            if (!message) return;

            // Tampilkan pesan user di UI
            addMessageToChat(message, "user");
            chatInput.value = "";

            const mode = chatMode.value;

            try {
                if (mode === "chat") {
                    // Create response box for AI message
                    const responseBox = document.createElement("div");
                    responseBox.classList.add("chat-message", "ai");
                    // Tambahkan style berikut untuk memelihara newline
                    responseBox.style.whiteSpace = "pre-wrap";  
                    chatMessages.appendChild(responseBox);

                    const res = await fetch("http://localhost:5282/api/ai/infer", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({ prompt: message })
                    });

                    if (!res.ok) {
                        throw new Error(`HTTP error! status: ${res.status}`);
                    }

                    const reader = res.body.getReader();
                    const decoder = new TextDecoder("utf-8");
                    let buffer = "";
                    let accumulatedText = "";

                    while (true) {
                        const { done, value } = await reader.read();
                        if (done) break;

                        // Decode new chunk and add to buffer
                        const chunk = decoder.decode(value, { stream: true });
                        buffer += chunk;

                        // Process buffer for complete JSON objects
                        while (buffer.length > 0) {
                            // Find the boundaries of a complete JSON object
                            let openBraces = 0;
                            let inString = false;
                            let escapeNext = false;
                            let jsonObjectEnd = -1;

                            for (let i = 0; i < buffer.length; i++) {
                                const char = buffer[i];

                                if (escapeNext) {
                                    escapeNext = false;
                                    continue;
                                }

                                if (char === '\\') {
                                    escapeNext = true;
                                    continue;
                                }

                                if (char === '"' && !escapeNext) {
                                    inString = !inString;
                                    continue;
                                }

                                if (!inString) {
                                    if (char === '{') {
                                        openBraces++;
                                    } else if (char === '}') {
                                        openBraces--;
                                        if (openBraces === 0) {
                                            jsonObjectEnd = i + 1;
                                            break;
                                        }
                                    }
                                }
                            }

                            // If we found a complete JSON object, process it
                            if (jsonObjectEnd > 0) {
                                const jsonStr = buffer.substring(0, jsonObjectEnd);
                                buffer = buffer.substring(jsonObjectEnd);

                                try {
                                    const data = JSON.parse(jsonStr);

                                    // Process the data
                                    if (data && Array.isArray(data.output)) {
                                        // Join array elements to form the content chunk
                                        const content = data.output.join('');

                                        if (content) {
                                            // Handle thinking mode with collapsible sections
                                            if (content.includes('<thinking>') || content.includes('</thinking>')) {
                                                handleThinkingMode(responseBox, content);
                                            } else {
                                                // Replace escaped newlines with actual newlines before accumulating
                                                const textFragment = content.replace(/\\n/g, '\n');
                                                const htmlFragment = textFragment.replace(/\n/g, '<br/>');
                                                accumulatedText += htmlFragment;
                                                responseBox.innerHTML = accumulatedText;
                                            }
                                            // Ensure the chat scrolls to the bottom and give the browser a chance to render
                                            chatMessages.scrollTop = chatMessages.scrollHeight;
                                            await new Promise((r) => setTimeout(r, 0));
                                        }
                                    }
                                } catch (parseError) {
                                    console.warn("Failed to parse JSON:", jsonStr, parseError);
                                }
                            } else {
                                // No complete JSON object found, wait for more data
                                break;
                            }
                        }
                    }

                    // Process any remaining data in buffer after stream ends
                    if (buffer.trim() !== "") {
                        try {
                            const data = JSON.parse(buffer);
                            if (data && Array.isArray(data.output)) {
                                const content = data.output.join('');
                                if (content) {
                                    // Handle thinking mode with collapsible sections
                                    if (content.includes('<thinking>') || content.includes('</thinking>')) {
                                        handleThinkingMode(responseBox, content);
                                    } else {
                                        // Replace escaped newlines with actual newlines before accumulating
                                        const textFragment = content.replace(/\\n/g, '\n');
                                        const htmlFragment = textFragment.replace(/\n/g, '<br/>');
                                        accumulatedText += htmlFragment;
                                        responseBox.innerHTML = accumulatedText;
                                    }
                                }
                            }
                        } catch (e) {
                            console.warn("Could not parse remaining buffer:", buffer);
                        }
                    }

                    // Scroll to bottom one last time
                    chatMessages.scrollTop = chatMessages.scrollHeight;
                } else {
                    // 🔹 Mode Agent (via API Workflow)
                    const res = await fetch(`/api/chat/send`, {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                            message: message
                        })
                    });

                    if (!res.ok) throw new Error("API Agent request failed");
                    const data = await res.json();
                    const responseText = data.output || "[no response from Agent]";
                    addMessageToChat(responseText, "ai");
                }
            } catch (err) {
                console.error("Error sending message:", err);
                addMessageToChat("⚠️ Error: " + err.message, "error");
            }
        }

        // Function to handle thinking mode with collapsible sections
        function handleThinkingMode(responseBox, content) {
            // Create or get the thinking container
            let thinkingContainer = responseBox.querySelector('.thinking-container');

            if (content.includes('<thinking>')) {
                // Create thinking container when <thinking> tag is detected
                thinkingContainer = document.createElement('div');
                thinkingContainer.className = 'thinking-container';
                thinkingContainer.innerHTML = `
                    <div class="thinking-header" style="cursor: pointer; background-color: #e9ecef; padding: 5px; border-radius: 3px; margin-top: 5px;">
                        <span class="toggle-icon">▼</span> Thinking...
                    </div>
                    <div class="thinking-content" style="padding: 5px; display: block;">
                        <div class="thinking-text" style="white-space: pre-wrap;"></div>
                    </div>
                `;

                // Add toggle functionality
                const header = thinkingContainer.querySelector('.thinking-header');
                const contentDiv = thinkingContainer.querySelector('.thinking-content');
                const toggleIcon = thinkingContainer.querySelector('.toggle-icon');

                header.addEventListener('click', () => {
                    if (contentDiv.style.display === 'none') {
                        contentDiv.style.display = 'block';
                        toggleIcon.textContent = '▼';
                    } else {
                        contentDiv.style.display = 'none';
                        toggleIcon.textContent = '►';
                    }
                });

                responseBox.appendChild(thinkingContainer);
            } else if (content.includes('</thinking>')) {
                // End of thinking section - do nothing special for now
                return;
            } else if (thinkingContainer) {
                // Add content to thinking container
                const thinkingText = thinkingContainer.querySelector('.thinking-text');
                if (thinkingText) {
                    // Replace escaped newlines with actual newlines for better readability
                    const textFragment = content.replace(/\\n/g, '\n');
                    const htmlFragment = textFragment.replace(/\n/g, '<br/>');
                    thinkingText.innerHTML += htmlFragment;
                }
            } else {
                // Regular content outside thinking blocks
                // Replace escaped newlines with actual newlines
                const textFragment = content.replace(/\\n/g, '\n');
                const htmlFragment = textFragment.replace(/\n/g, '<br/>');
                responseBox.innerHTML += htmlFragment;
            }
        }

        // Function to append message to chat UI
        function addMessageToChat(message, sender) {
            const messageElement = document.createElement("div");
            messageElement.classList.add("chat-message", sender);
            messageElement.textContent = message;

            chatMessages.appendChild(messageElement);
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }

        // Add initial welcome message
        setTimeout(() => {
            addMessageToChat('Hallo Bos besar! ada yang bisa saya bantu hari ini?', 'ai');
        }, 500);
    });
})();
