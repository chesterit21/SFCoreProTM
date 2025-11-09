// signalr-client.js (atau nama file Anda)

class AgentHubClient {
    constructor(hubUrl) {
        this.hubUrl = hubUrl;
        this.userId = this.getOrSetUserId();
        this.connection = this.buildConnection();
        this.registerEventHandlers();
    }

    /**
     * Mengambil userId dari sessionStorage atau membuat yang baru jika tidak ada.
     */
    getOrSetUserId() {
        let userId = sessionStorage.getItem('agentHubUserId');
        if (!userId) {
            userId = this.generateGuid();
            sessionStorage.setItem('agentHubUserId', userId);
        }
        console.log(`Using User ID: ${userId}`);
        return userId;
    }

    /**
     * Membangun koneksi SignalR dengan URL absolut dan konfigurasi.
     */
    buildConnection() {
        return new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl) // <-- Menggunakan URL absolut
            .withAutomaticReconnect([0, 2000, 5000, 10000, 15000])
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }

    /**
     * Mendaftarkan semua event handler untuk koneksi SignalR.
     */
    registerEventHandlers() {
        this.connection.on("AgentStarted", (data) => {
            console.log("Agent Started:", data);
            updateUI(data.message, "info");
        });

        this.connection.on("ProgressUpdate", (update) => {
            console.log("Progress:", update);
            updateProgress(update.progressPercentage, update.stage);
        });

        this.connection.on("TaskCreated", (task) => {
            console.log("Task Created:", task);
            addTaskToList(task);
        });

        this.connection.on("ProjectCreated", (data) => {
            console.log("Project Created:", data);
            updateUI(`Project selesai! ${data.taskCount} tasks generated.`, "success");
            // Reset progress bar setelah selesai
            setTimeout(() => updateProgress(0, "Menunggu task baru..."), 2000);
        });

        this.connection.on("AgentError", (error) => {
            console.error("Agent Error:", error);
            updateUI(`Error: ${error.error || 'Terjadi kesalahan.'}`, "danger");
        });
    }

    /**
     * Memulai koneksi ke hub.
     */
    async start() {
        try {
            await this.connection.start();
            console.log("SignalR Connected!");
            if (document.getElementById("connection-status")) {
                document.getElementById("connection-status").textContent = "Connected";
                document.getElementById("connection-status").className = "status-connected";
            }
        } catch (err) {
            //console.error("Initial connection failed:", err);
            if (document.getElementById("connection-status")) {
                document.getElementById("connection-status").textContent = "Disconnected";
                document.getElementById("connection-status").className = "status-disconnected";
            }
            // Automatic reconnect akan menangani koneksi yang terputus, 
            // tapi kita bisa mencoba lagi untuk koneksi awal yang gagal.
            setTimeout(() => {
                this.start();
            }, 5000);
        }
    }

    /**
     * Mengirim permintaan untuk memproses project.
     */
    async processProject(projectName, description, userMessage) {
        if (this.connection.state !== signalR.HubConnectionState.Connected) {
            updateUI("Koneksi terputus. Mencoba menyambungkan kembali...", "warning");
            await this.start(); // Coba sambungkan lagi jika belum terhubung
            if (this.connection.state !== signalR.HubConnectionState.Connected) {
                 updateUI("Gagal terhubung ke server.", "danger");
                 return;
            }
        }

        try {
            console.log(`Invoking ProcessProjectWithAgent for user: ${this.userId}`);
            await this.connection.invoke("ProcessProjectWithAgent", {
                projectName: projectName,
                projectDescription: description,
                userMessage: userMessage,
                userId: this.userId // <-- Menggunakan userId yang konsisten
            });
            updateUI("Permintaan terkirim. Menunggu respons dari agent...", "info");
        } catch (err) {
            console.error("Invoke error:", err);
            updateUI("Error saat mengirim permintaan: " + err.message, "danger");
        }
    }

    generateGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
            const r = Math.random() * 16 | 0;
            const v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

// --- Inisialisasi dan Penggunaan ---

// Ganti dengan URL backend Anda yang sebenarnya
const hubUrl = "http://localhost:5282/hubs/agentflow"; 
const agentClient = new AgentHubClient(hubUrl);

// Mulai koneksi saat dokumen siap
document.addEventListener('DOMContentLoaded', () => {
    agentClient.start();

    // Contoh event listener untuk form submit
    const form = document.getElementById("project-form");
    if (form) {
        form.addEventListener("submit", (e) => {
            e.preventDefault();
            const projectName = document.getElementById("project-name").value;
            const projectDesc = document.getElementById("project-description").value;
            const userMsg = document.getElementById("user-message").value;
            
            // Reset UI sebelum memulai
            updateProgress(0, "Mengirim permintaan...");
            
            agentClient.processProject(projectName, projectDesc, userMsg);
        });
    }
});


// --- Fungsi Helper untuk UI (tetap sama) ---

function updateUI(message, type) {
    // Implementasi UI Anda, contoh: menampilkan notifikasi
    console.log(`UI Update (${type}):`, message);
    const notificationArea = document.getElementById("notification-area");
    if (notificationArea) {
        notificationArea.textContent = message;
        notificationArea.className = `alert alert-${type}`;
    }
}

function updateProgress(percentage, message) {
    const progressBar = document.getElementById("progress-bar");
    const progressText = document.getElementById("progress-text");
    
    if (progressBar) {
        progressBar.style.width = percentage + "%";
        progressBar.setAttribute("aria-valuenow", percentage);
    }
    
    if (progressText) {
        progressText.textContent = `${message} (${percentage}%)`;
    }
}

function addTaskToList(task) {
    // Implementasi UI Anda untuk menambahkan task ke daftar
    console.log("Adding task to list:", task.taskTitle);
    const taskList = document.getElementById("task-list");
    if (taskList) {
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.textContent = `[${task.status}] ${task.taskTitle}`;
        taskList.appendChild(li);
    }
}
