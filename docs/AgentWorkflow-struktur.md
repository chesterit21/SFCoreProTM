SFCoreProTM.sln
│
├── src/
│   ├── SFCoreProTM/                          # ✅ Project utama (ASP.NET Core MVC)
│   │   ├── Application/                      # Clean Architecture layer
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   ├── WebUI/
│   │   └── SFCoreProTM.csproj
│   │
│   ├── SFCore.AgentWorkflow/                 # 🚀 Agent Orchestrator (Worker Service)
│   │   ├── Core/
│   │   │   ├── Agents/                       # AgentA, AgentB, AgentC...
│   │   │   ├── Orchestration/                # AgentPipeline, Scheduler, Context
│   │   │   ├── MCP/                          # MemoryClient, FileSystemClient, PlaywrightClient
│   │   │   ├── Services/                     # LLMConnector, ToolService, LoggingService
│   │   │   ├── Contracts/                    # DTO + interface antar modul
│   │   │   └── Core.csproj
│   │   │
│   │   ├── Infrastructure/
│   │   │   ├── Persistence/                  # EF Core context + repo (Task, FlowOfTask, AgentJob)
│   │   │   ├── Config/                       # appsettings, env var, options pattern
│   │   │   └── Extensions/                   # Dependency injection helper
│   │   │
│   │   ├── API/                              # (optional) expose API utk MAF callback
│   │   │   ├── Controllers/
│   │   │   ├── DTOs/
│   │   │   └── Middlewares/
│   │   │
│   │   ├── Worker/                           # BackgroundService: Scheduler & Dispatcher
│   │   └── SFCore.AgentWorkflow.csproj
│   │
│   ├── SFCore.Shared/                        # 🔗 Shared contracts antar project
│   │   ├── Events/                           # Domain event (TaskCreated, TaskCompleted)
│   │   ├── DTOs/                             # Common DTO (Project, Task, ERD, FlowOfTask)
│   │   ├── Enums/                            # Status enum, AgentType, StepKind, dll
│   │   └── SFCore.Shared.csproj
│   │
│   └── SFCore.Logging/                       # (optional) logging cross-cutting lib
│       └── SFCore.Logging.csproj
│
├── tests/
│   ├── SFCoreProTM.Tests/
│   ├── SFCore.AgentWorkflow.Tests/
│   └── SFCore.Shared.Tests/
│
└── docs/
    ├── architecture.md
    ├── agent-sequence-flow.md
    └── erd-definition-spec.md
