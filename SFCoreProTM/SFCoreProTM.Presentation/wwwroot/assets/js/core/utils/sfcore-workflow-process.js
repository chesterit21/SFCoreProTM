// Fungsi untuk generate thinking text berdasarkan input user
const ThinkingTextMapper = {
    // Keyword mapping untuk deteksi tipe tugas
    taskKeywords: {
        project: ['proyek', 'project', 'aplikasi', 'sistem', 'system'],
        module: ['modul', 'module', 'fitur', 'feature', 'komponen'],
        task: ['task', 'tugas', 'pekerjaan', 'pekerjaan', 'aktivitas'],
        erd: ['erd', 'database', 'basis data', 'model data', 'entity', 'tabel'],
        flow: ['flow', 'alur', 'proses', 'workflow', 'diagram'],
        all: ['semua', 'semuanya', 'semua nya', 'semuanya', 'semua nya', 'semua']
    },

    // Template thinking text yang abstrak
    thinkTemplates: [
        `hmmm...saya diminta untuk melakukan tugas dari Pak Bos. Pertama-tama saya perlu memahami konteksnya...

Melakukan deep analysis terhadap landscape requirement... Mencari pola dan hubungan antara berbagai dimensi sistem...

Mapping the architectural territory... Menjelajahi kemungkinan solusi yang scalable dan maintainable...

Mendekonstruksi complex requirements menjadi foundational components... Menyusun ulang menjadi coherent architecture...

Navigating the possibility space untuk menemukan optimal solution path...`,

        `hmmm...saya diminta untuk melakukan tugas dari Pak Bos. Pertama-tama saya perlu memahami konteksnya...

Memetakan cognitive map dari problem domain... Menganalisa interaksi antara berbagai layer abstraction...

Merancang symbiotic relationships antara business logic dan technical implementation...

Menciptakan dynamic equilibrium antara functional requirements dan non-functional constraints...

Menyelaraskan cosmic purpose dengan practical execution reality...`,

        `hmmm...saya diminta untuk melakukan tugas dari Pak Bos. Pertama-tama saya perlu memahami konteksnya...

Exploring multidimensional aspects dari solution space... Mencari harmonic convergence antara berbagai components...

Membangun bridge antara abstract vision dengan concrete implementation details...

Orchestrating symphony of system interactions dalam holistic ecosystem...

Transcending traditional boundaries untuk mencapai innovative solution...`
    ],

    // Deteksi tipe tugas dari input user
    detectTaskTypes(userInput) {
        const input = userInput.toLowerCase();
        const detectedTypes = new Set();

        for (const [taskType, keywords] of Object.entries(this.taskKeywords)) {
            if (keywords.some(keyword => input.includes(keyword))) {
                detectedTypes.add(taskType);
            }
        }

        return Array.from(detectedTypes);
    },

    // Generate thinking text berdasarkan tipe tugas yang terdeteksi
    generateThinkText(userInput) {
        const detectedTypes = this.detectTaskTypes(userInput);
        
        // Jika user minta "semua" atau multiple types, gunakan template yang lebih komprehensif
        if (detectedTypes.includes('all') || detectedTypes.length >= 3) {
            return this.generateComprehensiveThink();
        }

        // Jika specific types terdeteksi, generate accordingly
        if (detectedTypes.length > 0) {
            return this.generateTargetedThink(detectedTypes);
        }

        // Default generic thinking text
        return this.thinkTemplates[Math.floor(Math.random() * this.thinkTemplates.length)];
    },

    // Generate thinking text untuk tugas komprehensif
    generateComprehensiveThink() {
        return `hmmm...saya diminta untuk melakukan tugas dari Pak Bos. Pertama-tama saya perlu memahami konteksnya...

Melakukan holistic analysis terhadap entire project ecosystem... Mapping end-to-end architecture dari conceptualization sampai implementation...

Mendekomposisi grand vision menjadi manageable components... Merancang scalable foundation untuk future growth...

Menciptakan coherent integration antara semua system layers... Memastikan architectural integrity across all dimensions...

Navigating complex interdependencies antara berbagai modules dan workflows...`;
    },

    // Generate thinking text untuk tipe tugas spesifik
    generateTargetedThink(detectedTypes) {
        const aspects = [];
        
        if (detectedTypes.includes('project')) {
            aspects.push("menganalisis project architecture dan scalability requirements");
        }
        if (detectedTypes.includes('module')) {
            aspects.push("mendesain modular components dengan clear boundaries");
        }
        if (detectedTypes.includes('task')) {
            aspects.push("memetakan task workflows dan business processes");
        }
        if (detectedTypes.includes('erd')) {
            aspects.push("merancang data model relationships dan integrity constraints");
        }
        if (detectedTypes.includes('flow')) {
            aspects.push("mengorkestrasi process flows dan system interactions");
        }

        const baseThink = `hmmm...saya diminta untuk melakukan tugas dari Pak Bos. Pertama-tama saya perlu memahami konteksnya...

Saya mendeteksi kebutuhan untuk ${aspects.join(', ')}...

Melakukan deep dive analysis terhadap requirement space... Mencari optimal patterns untuk implementation...

Mapping conceptual framework terhadap technical execution...`;

        return baseThink;
    }
};

// Update SFCoreWorkflowProcess dengan fungsi mapping
class SFCoreWorkflowProcess {
    startTransitionAgent(userInput = "") {
        // Generate thinking text berdasarkan input user
        let thinkMapper = ThinkingTextMapper.generateThinkText(userInput);
        
        // Wrap dengan tag think
        thinkMapper = `<think>${thinkMapper}</think>`;

        console.log('Generated think text:', thinkMapper); // Debug log

        ChatTextAppender.appendTextToChat(thinkMapper, {
            thinkingMode: true,
            streaming: true,
            streamDelay: 25
        });

        // Generate follow-up messages berdasarkan tipe tugas
        const detectedTypes = ThinkingTextMapper.detectTaskTypes(userInput);
        const followUpMessages = this.generateFollowUpMessages(detectedTypes);
        setTimeout(() => {
            ChatTextAppender.appendMultipleMessages(followUpMessages, {
                streaming: true,
                delayBetweenMessages: 1200
            });
        }, 16000);


    }

    // Generate follow-up messages berdasarkan tipe tugas
    generateFollowUpMessages(detectedTypes) {
        const messages = [];
        
        // Always start with analysis phase
        messages.push('Memulai analisis mendalam terhadap requirements...');
        
        // Add specific phases based on detected types
        if (detectedTypes.includes('project') || detectedTypes.includes('all')) {
            messages.push('Menyusun project architecture dan foundation...');
            messages.push('Configure base dependencies dan project structure...');
        }
        
        if (detectedTypes.includes('module') || detectedTypes.includes('all')) {
            messages.push('Mendesain modular components dengan clear contracts...');
            messages.push('Implement business logic dan service layers...');
        }
        
        if (detectedTypes.includes('task') || detectedTypes.includes('all')) {
            messages.push('Mapping task workflows dan business processes...');
            messages.push('Implement task execution dan state management...');
        }
        
        if (detectedTypes.includes('erd') || detectedTypes.includes('all')) {
            messages.push('Merancang database schema dan relationships...');
            messages.push('Define entity models dan data access patterns...');
        }
        
        if (detectedTypes.includes('flow') || detectedTypes.includes('all')) {
            messages.push('Mengorkestrasi process flows dan system interactions...');
            messages.push('Implement workflow engine dan state transitions...');
        }
        
        // Always end with integration
        messages.push('Melakukan integration testing dan quality assurance...');
        messages.push('Finalizing solution dengan optimal performance...');
        messages.push('Baik saya akan melaksakkan prosesnya dengan benar dan saya harus bersungguh-sungguh.');
        messages.push('Sekarang saat nya saya mengambil alih penggunaan aplikasi ini.');
        messages.push('Siap Pak Bos, saya akan memulainya.');

        return messages;
    }
}

class SFCoreCallingAgent{

    initializeUserInputHandler(userInput = "") {
        const workflow = new SFCoreWorkflowProcess();
        // User input berbeda-beda akan menghasilkan thinking text yang berbeda
        workflow.startTransitionAgent(userInput);

        setTimeout(() => {
            sfcoreStorage.set("inisiateTaskUser", userInput);
            window.location.href = '/ProjectsMenu/Create';
        }, 32000);        
    }
}

// Make it globally accessible
window.ThinkingTextMapper = ThinkingTextMapper;
window.SFCoreWorkflowProcess = SFCoreWorkflowProcess;
window.SFCoreCallingAgent = SFCoreCallingAgent;