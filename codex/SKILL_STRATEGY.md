# Skill Strategy

## Existing Skills

`CodexCommonAgents/skills`: NOT_AVAILABLE.

Reusable skill behavior exists as playbooks/rules, not packaged skills:

| Source | Status | Use |
|---|---|---|
| `playbooks/codex-low-token-mode.md` | REUSE | Context minimization |
| `playbooks/portal-first-implementation.md` | REUSE | Portal classification |
| `playbooks/master-agent-orchestration.md` | REUSE | Agent selection |
| `registry/reusable-portal-apis.md` | REUSE | Portal capability registry |
| `registry/do-not-duplicate.md` | REUSE | Duplication guardrail |

## Domain Skills

| Skill | Status | Purpose | Consumer Agent | Inputs | Outputs | Dependencies | External Provider | Max Context | Sprint |
|---|---|---|---|---|---|---|---|---|---|
| channel-strategy | CREATE | Define channel goals and constraints | ChannelStrategyAgent | channel, audience | strategy JSON | Configuration | none | S | 1 |
| content-research | DEFER | Gather safe trends/topics | ChannelStrategyAgent | niche, locale | research JSON | policy | ITextGenerationProvider later | M | 1 |
| story-ideation | CREATE | Propose episode ideas | StoryStrategyAgent | bible, strategy | ideas JSON | channel-strategy | ITextGenerationProvider | M | 2 |
| story-bible | CREATE | Maintain universe facts | StoryStrategyAgent | characters, places | bible JSON | Configuration | none | M | 2 |
| character-consistency | CREATE | Validate character continuity | Quality and Child Safety | bible, script | findings JSON | story-bible | none | M | 3 |
| episode-generation | CREATE | Generate episode draft | StoryGenerationAgent | idea, bible | draft JSON | script-json | ITextGenerationProvider | M | 3 |
| script-json | CREATE | Enforce structured script contract | StoryGenerationAgent | draft | versioned JSON | JSON schema | none | S | 3 |
| continuity-check | CREATE | Check timeline/visual continuity | Quality and Child Safety | script,bible | findings JSON | story-bible | none | M | 3 |
| semantic-originality | CREATE | Detect repetition/IP risk | Quality and Child Safety | script,history | risk JSON | history store | ITextGenerationProvider optional | M | 3 |
| scene-plan | CREATE | Convert script to scenes | ScenePlannerAgent | script JSON | scene plan JSON | script-json | none | M | 3 |
| visual-prompt | CREATE | Generate provider-neutral image prompts | VisualPromptAgent | scene plan,bible | prompts JSON | policy-check | IImageGenerationProvider later | M | 4 |
| voice-direction | DEFER | Voice/cadence instructions | MediaProductionAgent | script,characters | voice JSON | story-bible | IVoiceGenerationProvider later | M | 4 |
| image-generation | DEFER | Produce images via adapter | AI Media Integrator | prompts,budget | asset manifest | CostControlAgent | IImageGenerationProvider | L | 4 |
| video-generation | DEFER | Produce clips via adapter | AI Media Integrator | images,scene plan | clip manifest | CostControlAgent | IVideoGenerationProvider | L | 4 |
| media-license | CREATE | Track asset license/source | Media Pipeline | asset manifest | license report | storage | none | S | 4 |
| cost-estimation | CREATE | Estimate before providers | CostControlAgent | plan,provider rates | ALLOW/WARN/BLOCK | provider catalog | none | S | 4 |
| ffmpeg-render | CREATE | Render deterministic outputs | Media Pipeline | clip/audio/subtitles | render manifest | FFmpeg | IRenderProvider | M | 5 |
| thumbnail | CREATE | Create thumbnail spec/output | Media Pipeline | episode,assets | thumbnail manifest | policy-check | IRenderProvider | S | 5 |
| technical-quality | CREATE | Validate media specs | Media Pipeline | render manifest | QA JSON | ffprobe | none | M | 5 |
| policy-check | CREATE | Child safety/IP/publish blocks | Quality and Child Safety | script/media/meta | policy JSON | rules | none | M | 6 |
| human-approval | CREATE | Prepare approval packet | HumanApprovalAgent | QA,cost,preview | approval request | Portal Notification | none | S | 6 |
| youtube-publish | DEFER | Upload approved publication | Publishing and Operations | ApprovedPublication | publish result | human-approval | ISocialPublishingProvider | L | 7 |
| tiktok-publish | DEFER | Future social adapter | Publishing and Operations | ApprovedPublication | publish result | approval | ISocialPublishingProvider | L | 9 |
| meta-publish | DEFER | Future Meta adapter | Publishing and Operations | ApprovedPublication | publish result | approval | ISocialPublishingProvider | L | 9 |
| metrics-analysis | CREATE | Analyze performance | AnalyticsLearningAgent | metrics | insights JSON | analytics store | IAnalyticsProvider | M | 8 |
| next-content-recommendation | CREATE | Recommend next topic | AnalyticsLearningAgent | insights,bible | recommendation JSON | metrics-analysis | ITextGenerationProvider optional | M | 8 |

## Provider Interfaces

Future adapters must implement interfaces: `ITextGenerationProvider`, `IImageGenerationProvider`, `IVideoGenerationProvider`, `IVoiceGenerationProvider`, `IRenderProvider`, `ISocialPublishingProvider`, `IMediaStorage`, `IAnalyticsProvider`.

Agents must depend on domain interfaces and never on `HiggsfieldAgent`, `OpenAIAgent`, `SeedanceAgent`, or social SDKs directly.
