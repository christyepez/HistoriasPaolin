# Agent Matrix

## Available Common Agents

| Name | Path | Purpose | Can Reuse | Reviewer Candidate | Context Cost |
|---|---|---|---|---|---|
| Coordinator Agent | `CodexCommonAgents/agents/00-coordinator-agent.md` | Scope, dependencies, Portal classification | Yes | No | XS |
| Portal Reuse Agent | `CodexCommonAgents/agents/01-portal-reuse-agent.md` | REUSE/EXTEND/ADAPT decisions | Yes | Yes | XS |
| Solution Architect Agent | `CodexCommonAgents/agents/02-solution-architect-agent.md` | Architecture, bounded contexts, contracts | Yes | Yes | XS |

## Selected Common Agents

| Common Need | Selected Agent | Relevant Tasks |
|---|---|---|
| SolutionArchitectureAgent | Solution Architect Agent | HP-101, HP-201, HP-301, HP-401 |
| VerticalArchitectureAgent | Solution Architect Agent + Backend Architect | HP-101..HP-205 |
| IntegrationAgent | Portal Reuse Agent + Backend Architect | HP-0A03, HP-0A04, HP-401 |
| SecurityAgent | Portal Reuse Agent | HP-0A03, HP-601, HP-701 |
| CodeReviewAgent | Solution Architect Agent | Review tasks |
| DocumentationAgent | Coordinator Agent | Sprint close, docs |

## Local Agents

| Agent | Path | Responsibility | Sprint | Reviewer | Budget |
|---|---|---|---|---|---|
| Sprint Orchestrator | `.codex/agents/orchestrator.md` | Select task, prerequisites, evidence | All | Solution Architect | XS |
| Backend Architect | `.codex/agents/backend-architect.md` | .NET, SQL Server, APIs, Docker | 0-3, 8-10 | Solution Architect | M |
| Quality and Child Safety | `.codex/agents/quality-policy.md` | Policy, originality, script/media quality | 3, 6 | Solution Architect | M |
| Media Pipeline | `.codex/agents/media-pipeline.md` | FFmpeg, render, technical outputs | 5 | Quality and Child Safety | M |
| AI Media Integrator | `.codex/agents/ai-media-integrator.md` | Provider adapters, dry-run, cost gates | 4 | Quality and Child Safety | M |
| Publishing and Operations | `.codex/agents/publishing-operations.md` | YouTube/Ops after approval | 7-10 | Quality and Child Safety | M |

## Domain Agent Classification

| Proposed Agent | Decision | Use Existing / Create Later | Sprint |
|---|---|---|---|
| ChannelStrategyAgent | CREATE | Backlog small domain agent | 1 |
| ContentResearchAgent | MERGE | ChannelStrategyAgent until repeated need grows | 1 |
| StoryStrategyAgent | CREATE | Backlog small domain agent | 2 |
| StoryGenerationAgent | CREATE | Backlog small domain agent | 3 |
| StoryContinuityAgent | MERGE | Quality and Child Safety initially | 3,6 |
| OriginalityAgent | MERGE | Quality and Child Safety initially | 3,6 |
| ScriptReviewAgent | MERGE | Quality and Child Safety initially | 3,6 |
| ScenePlannerAgent | CREATE | Backlog small domain agent | 3 |
| VisualPromptAgent | CREATE | Backlog small domain agent | 4 |
| VoiceDirectorAgent | DEFER | Combine with MediaProduction until voice scope repeats | 4 |
| MediaProductionAgent | MERGE | AI Media Integrator initially | 4 |
| RenderAgent | MERGE | Media Pipeline | 5 |
| PolicyComplianceAgent | MERGE | Quality and Child Safety | 6 |
| HumanApprovalAgent | CREATE | Backlog small domain agent | 6 |
| PublishingAgent | MERGE | Publishing and Operations | 7 |
| CostControlAgent | CREATE | Backlog small domain agent | 4 |
| AnalyticsLearningAgent | CREATE | Backlog small domain agent | 8 |

## Sprint Task Map

| TaskId | Sprint | PrimaryAgent | ReviewerAgent | Rule | Playbook | Skill | PortalCapability | Provider | FilesAllowed | FilesForbidden | Validation | Budget |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| HP-001 | 0 | Backend Architect | Solution Architect | Common .NET | low-token | none | none | none | `src/`, `tests/` | `.env`, generated | build,test | S |
| HP-002 | 0 | Backend Architect | Solution Architect | logs/analyzers | low-token | observability | Correlation | none | `src/`, docs | secrets | build,test | S |
| HP-003 | 0 | Backend Architect | Portal Reuse | Docker | low-token | docker | Gateway | none | Dockerfiles, compose | `.env` | compose,health | M |
| HP-004 | 0 | Backend Architect | none | SQL | low-token | sqlserver | none | SQL Server | scripts,database | Portal DB | sql smoke | S |
| HP-005 | 0 | Backend Architect | none | health | low-token | health | Health | none | API, tests | secrets | health,test | S |
| HP-006 | 0 | Backend Architect | none | idempotent DB | low-token | sqlserver | none | SQL Server | database,scripts | Portal DB | sql smoke | S |
| HP-0A01 | 0A | Portal Reuse Agent | Solution Architect | Portal-first | portal-first | portal-reuse | all | none | docs,codex | Portal source broad | docs | S |
| HP-0A02 | 0A | Backend Architect | Portal Reuse | Gateway auth | portal-first | gateway-smoke | Gateway,Security | none | compose,Gateway docs | Portal DB | smoke | M |
| HP-0A03 | 0A | Portal Reuse Agent | Solution Architect | least privilege | portal-first | portal-onboarding | Security,Menu,Configuration | none | docs,adapters | Portal DB | smoke | M |
| HP-0A04 | 0A | Backend Architect | Portal Reuse | adapters | portal-first | audit-notify | Audit,Notification | none | `src/*Portal*`, tests | Portal DB | test | M |
| HP-0A05 | 0A | Sprint Orchestrator | Solution Architect | evidence | low-token | integrated-smoke | all | none | scripts,docs | secrets | smoke | M |
| HP-101 | 1 | Backend Architect | Solution Architect | DDD | low-token | domain-model | none | none | domain,app,tests | infra broad | test | M |
| HP-102 | 1 | Backend Architect | Quality | states | low-token | state-machine | none | none | domain,tests | providers | test | S |
| HP-103 | 1 | Backend Architect | none | EF SQL | low-token | sqlserver | none | SQL Server | infra,migrations | Portal DB | test,sql | M |
| HP-104 | 1 | Backend Architect | Solution Architect | audit/concurrency | low-token | persistence | Audit adapter future | SQL Server | infra,tests | Portal DB | test | M |
| HP-105 | 1 | Backend Architect | none | integration tests | low-token | testing | none | SQL Server | tests,scripts | secrets | test | M |
| HP-201..HP-205 | 2 | Sprint Orchestrator | Backend Architect | orchestration | low-token | pipeline | Audit/Notification | none | app,worker,tests | providers | test | M |
| HP-301..HP-305 | 3 | StoryGenerationAgent | Quality | JSON/policy | low-token | script-json | Configuration | ITextGenerationProvider | prompts,app,tests | providers real | test | M |
| HP-401..HP-405 | 4 | AI Media Integrator | CostControlAgent | adapters only | low-token | visual/video | Content future | image/video/voice interfaces | app,infra,tests | credentials | dry-run tests | L |
| HP-501..HP-505 | 5 | Media Pipeline | Quality | deterministic render | low-token | ffmpeg-render | Content future | IRenderProvider | media pipeline,tests | real media large | tests | M |
| HP-601..HP-605 | 6 | Quality and Child Safety | HumanApprovalAgent | approval gate | low-token | policy-check | Audit | none | quality,docs,tests | publish code | test | M |
| HP-701..HP-705 | 7 | Publishing and Operations | Quality | approval required | low-token | youtube-publish | Audit,Notification | ISocialPublishingProvider | publishing,tests | tokens | dry-run tests | L |
| HP-801..HP-805 | 8 | Publishing and Operations | Backend Architect | scheduling | low-token | automation | Notification | n8n/.NET | worker,scripts | n8n real runs | tests | M |
| HP-901..HP-905 | 9 | Publishing and Operations | Solution Architect | observability | low-token | observability | Seq,Audit | none | ops,docs,ci | secrets | build/test | M |
| HP-1001..HP-1005 | 10 | Sprint Orchestrator | Quality | pilot approval | low-token | sprint-close | all | dry-run only | docs,scripts | publish tokens | smoke | L |

No task is XL; grouped ranges must be split into individual tasks before implementation.
