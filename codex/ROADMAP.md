# Roadmap

Status values: DONE, PARTIAL, READY, BLOCKED, DEFERRED, NOT_REQUIRED.

## Sprint Objectives

| Sprint | Objective |
|---|---|
| 0 | Foundation: .NET, API, Worker, SQL, Docker, health, Gateway, security, smoke |
| 1 | Channels and editorial strategy |
| 2 | Series, characters and continuity |
| 3 | Ideas, scripts and originality |
| 4 | Scene planning and multimedia interfaces |
| 5 | Render and technical quality |
| 6 | Approval and costs |
| 7 | Publishing |
| 8 | Analytics |
| 9 | n8n automation |
| 10 | Multichannel scaling |

## Tasks

| TaskId | Sprint | Status | DependsOn | Blocks | PrimaryAgent | Skill | PortalCapability | Budget | AcceptanceCriteria |
|---|---:|---|---|---|---|---|---|---|---|
| HP-001 | 0 | DONE | none | HP-002 | Backend Architect | none | none | S | Solution/projects exist |
| HP-002 | 0 | DONE | HP-001 | HP-003 | Backend Architect | observability | Correlation | S | Logs/correlation baseline validated |
| HP-003 | 0 | DONE | HP-002 | HP-004 | Backend Architect | docker | Gateway | M | Docker API/Worker/migrations operational |
| HP-004 | 0 | DONE | HP-003 | HP-005 | Backend Architect | sqlserver | none | S | SQL diagnostics and startup validated |
| HP-005 | 0 | DONE | HP-004 | HP-006 | Backend Architect | health | Health | S | Health endpoints validated |
| HP-006 | 0 | DONE | HP-005 | HP-0A01 | Backend Architect | sqlserver | none | S | HistoriasPaolinDb idempotent migration applied |
| HP-0A01 | 0 | DONE | HP-006 | HP-0A02 | Portal Reuse Agent | portal-reuse | all | S | Reuse matrix documented |
| HP-0A02 | 0 | DONE | HP-0A01 | HP-0A03 | Backend Architect | gateway-smoke | Gateway,Security | M | Gateway auth smoke OK |
| HP-0A03 | 0 | PARTIAL | HP-0A02 | HP-101 | Portal Reuse Agent | portal-onboarding | Security,Menu,Configuration | M | Security validated; Menu/Configuration pending |
| HP-0A04 | 0 | PARTIAL | HP-0A02 | HP-604,HP-605 | Backend Architect | audit-notify | Audit,Notification | M | Adapter decision documented; full adapters pending |
| HP-0A05 | 0 | DONE | HP-0A02 | HP-101 | Sprint Orchestrator | integrated-smoke | all | M | Integrated smoke PASS 7/0/0 |
| HP-101 | 1 | DONE | HP-006,HP-0A02 | HP-102,HP-201 | Backend Architect | channel-strategy | Configuration | M | Channel CRUD persists in HistoriasPaolinDb |
| HP-102 | 1 | DONE | HP-101 | HP-103 | ChannelStrategyAgent | channel-strategy | Configuration | S | Editorial strategy persists and governs channel content rules |
| HP-103 | 1 | DONE | HP-101,HP-102 | HP-104 | Backend Architect | sqlserver | none | M | EF ModelSnapshot synchronized; migrations/order/indexes hardened |
| HP-104 | 1 | DONE | HP-103 | HP-105 | Backend Architect | persistence | Audit future | M | Concurrency and audit fields defined; CreatedAt indexes added |
| HP-105 | 1 | DONE | HP-104 | HP-201 | Backend Architect | testing | none | M | Sprint 1 domain/API/SQL Server integration coverage completed |
| HP-201 | 2 | READY | HP-105 | HP-202 | StoryStrategyAgent | story-bible | Configuration | M | SeriesBible aggregate exists |
| HP-202 | 2 | BLOCKED | HP-201 | HP-203 | StoryStrategyAgent | story-bible | Configuration | M | Characters and relationships persist |
| HP-203 | 2 | BLOCKED | HP-202 | HP-204 | StoryStrategyAgent | story-bible | Configuration | M | Narrative rules and universe metadata persist |
| HP-204 | 2 | BLOCKED | HP-203 | HP-205 | StoryStrategyAgent | continuity-check | none | M | Continuity memory records used elements |
| HP-205 | 2 | BLOCKED | HP-204 | HP-301 | Backend Architect | testing | none | M | Series/continuity tests pass |
| HP-301 | 3 | BLOCKED | HP-205 | HP-302 | ChannelStrategyAgent | content-research | Configuration | M | Research contract/dry-run output created |
| HP-302 | 3 | BLOCKED | HP-301 | HP-303 | StoryGenerationAgent | story-ideation | none | M | Ideas are scored and selectable |
| HP-303 | 3 | BLOCKED | HP-302 | HP-304 | StoryGenerationAgent | episode-generation | none | M | Script JSON schema versioned |
| HP-304 | 3 | BLOCKED | HP-303 | HP-305 | Quality and Child Safety | semantic-originality | none | M | Similarity/repetition gate returns findings |
| HP-305 | 3 | BLOCKED | HP-304 | HP-401 | Quality and Child Safety | policy-check | Configuration | M | Editorial/child policy gate blocks unsafe drafts |
| HP-401 | 4 | BLOCKED | HP-305 | HP-402 | ScenePlannerAgent | scene-plan | none | M | ScenePlan created from script |
| HP-402 | 4 | BLOCKED | HP-401 | HP-403 | VisualPromptAgent | visual-prompt | Content future | M | Provider-neutral prompts created |
| HP-403 | 4 | BLOCKED | HP-402 | HP-404 | AI Media Integrator | media-license | Content future | M | Media metadata/license/hash tracked |
| HP-404 | 4 | DEFERRED | HP-403,HP-605 | HP-405 | AI Media Integrator | image-generation | Content future | L | Image/video/voice adapters dry-run only |
| HP-405 | 4 | DEFERRED | HP-404 | HP-501 | CostControlAgent | cost-estimation | none | M | Provider cost gate returns ALLOW/WARN/BLOCK |
| HP-501 | 5 | BLOCKED | HP-405 | HP-502 | Media Pipeline | ffmpeg-render | Content future | M | RenderPlan/Timeline exists |
| HP-502 | 5 | BLOCKED | HP-501 | HP-503 | Media Pipeline | ffmpeg-render | none | M | 16:9 render manifest produced |
| HP-503 | 5 | BLOCKED | HP-502 | HP-504 | Media Pipeline | ffmpeg-render | none | M | Audio normalized/mixed |
| HP-504 | 5 | BLOCKED | HP-503 | HP-505 | Media Pipeline | thumbnail | none | M | Subtitles and thumbnail manifest produced |
| HP-505 | 5 | BLOCKED | HP-504 | HP-601 | Media Pipeline | technical-quality | none | M | 9:16 short and ffprobe manifest validated |
| HP-601 | 6 | BLOCKED | HP-505 | HP-602 | CostControlAgent | cost-estimation | Audit | M | Channel/episode budgets enforced |
| HP-602 | 6 | BLOCKED | HP-601 | HP-603 | HumanApprovalAgent | human-approval | Notification | M | Approval request packet created |
| HP-603 | 6 | BLOCKED | HP-602 | HP-604 | HumanApprovalAgent | human-approval | Audit | M | Reject/regenerate state transitions persist |
| HP-604 | 6 | BLOCKED | HP-603,HP-0A04 | HP-605 | Quality and Child Safety | policy-check | Audit | M | Approval/policy events auditable |
| HP-605 | 6 | BLOCKED | HP-604 | HP-701 | Publishing and Operations | policy-check | Notification | M | Publication blocked until approved |
| HP-701 | 7 | BLOCKED | HP-605 | HP-702 | Publishing and Operations | youtube-publish | Audit,Notification | M | Common publication contract exists |
| HP-702 | 7 | DEFERRED | HP-701 | HP-703 | Publishing and Operations | youtube-publish | Audit | L | YouTube adapter dry-run; OAuth real deferred |
| HP-703 | 7 | DEFERRED | HP-701 | HP-704 | Publishing and Operations | tiktok-publish | Audit | L | TikTok adapter contract only |
| HP-704 | 7 | DEFERRED | HP-701 | HP-705 | Publishing and Operations | meta-publish | Audit | L | Meta adapter contract only |
| HP-705 | 7 | BLOCKED | HP-702,HP-703,HP-704 | HP-801 | Publishing and Operations | automation | Audit,Notification | M | Idempotent states/retries/webhooks/deadletter |
| HP-801 | 8 | BLOCKED | HP-705 | HP-802 | AnalyticsLearningAgent | metrics-analysis | none | M | Metrics ingestion contract exists |
| HP-802 | 8 | BLOCKED | HP-801 | HP-803 | AnalyticsLearningAgent | metrics-analysis | none | M | Views/watch/retention/CTR captured |
| HP-803 | 8 | BLOCKED | HP-802 | HP-804 | AnalyticsLearningAgent | metrics-analysis | none | M | Cost/revenue/margin by channel captured |
| HP-804 | 8 | BLOCKED | HP-803 | HP-805 | AnalyticsLearningAgent | next-content-recommendation | none | M | Recommendations use metrics and bible |
| HP-805 | 8 | BLOCKED | HP-804 | HP-901 | Publishing and Operations | observability | Notification | M | Analytics dashboard data contract exists |
| HP-901 | 9 | BLOCKED | HP-805 | HP-902 | Publishing and Operations | automation | Notification | M | n8n daily-planning/generate-idea workflows specified |
| HP-902 | 9 | BLOCKED | HP-901 | HP-903 | Publishing and Operations | automation | Notification | M | Script/assets/render/quality/approval workflows specified |
| HP-903 | 9 | BLOCKED | HP-902 | HP-904 | Publishing and Operations | automation | Notification | M | Schedule/retry-publication workflows specified |
| HP-904 | 9 | BLOCKED | HP-903 | HP-905 | Publishing and Operations | automation | Notification | M | collect-metrics workflow specified |
| HP-905 | 9 | BLOCKED | HP-904 | HP-1001 | Publishing and Operations | observability | Seq,Audit | M | Rules stay in .NET; n8n orchestrates only |
| HP-1001 | 10 | BLOCKED | HP-905 | HP-1002 | Sprint Orchestrator | sprint-close | all | L | 3-channel MVP plan validated |
| HP-1002 | 10 | BLOCKED | HP-1001 | HP-1003 | Sprint Orchestrator | sprint-close | all | L | 7-day approved queue modeled |
| HP-1003 | 10 | BLOCKED | HP-1002 | HP-1004 | Publishing and Operations | automation | all | L | Daily per-channel schedule throttled |
| HP-1004 | 10 | BLOCKED | HP-1003 | HP-1005 | Publishing and Operations | observability | all | L | Circuit breakers and worker priorities specified |
| HP-1005 | 10 | BLOCKED | HP-1004 | none | Sprint Orchestrator | sprint-close | all | L | Executive dashboard and 5-10 channel scale decision |

L tasks reduced where natural: HP-401, HP-402, HP-403, HP-405, HP-701 and HP-705 are M after splitting provider and publishing work.
