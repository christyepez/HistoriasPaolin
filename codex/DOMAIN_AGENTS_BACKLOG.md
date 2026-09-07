# Domain Agents Backlog

No domain agent files are created by PROMPT 05. Create them only when the first sprint task needs them.

| AgentId | Purpose | Sprint | Dependencies | Skills | Status | Acceptance Criteria |
|---|---|---|---|---|---|---|
| ChannelStrategyAgent | Define audience, formats, cadence and budget per channel | 1 | Configuration EXTEND | channel-strategy, content-research | CREATE_LATER | Returns versioned channel strategy JSON; no external calls required |
| StoryStrategyAgent | Maintain story bible, themes and episode idea selection | 2 | ChannelStrategyAgent | story-bible, story-ideation | CREATE_LATER | Produces bible/idea JSON; avoids franchises and protected IP |
| StoryGenerationAgent | Generate structured episode script drafts | 3 | StoryStrategyAgent | episode-generation, script-json | CREATE_LATER | Produces schema-valid script JSON with prompt version |
| ScenePlannerAgent | Transform script into scene plan | 3 | StoryGenerationAgent | scene-plan | CREATE_LATER | Produces scene list with duration, assets, continuity references |
| VisualPromptAgent | Produce provider-neutral visual prompts | 4 | ScenePlannerAgent, StoryStrategyAgent | visual-prompt | CREATE_LATER | Outputs prompt JSON independent of Higgsfield/Seedance |
| CostControlAgent | Gate provider usage before paid work | 4 | provider cost catalog | cost-estimation | CREATE_LATER | Returns ALLOW/WARN/BLOCK without provider calls |
| HumanApprovalAgent | Prepare human approval packet | 6 | Quality review, cost result | human-approval | CREATE_LATER | Summarizes quality/cost/policy/preview; never auto-approves |
| AnalyticsLearningAgent | Turn metrics into next recommendations | 8 | analytics provider adapter | metrics-analysis, next-content-recommendation | CREATE_LATER | Produces insight/recommendation JSON with evidence |

## Merged Into Existing Agents

| Proposed Agent | Existing Owner | Reason |
|---|---|---|
| ContentResearchAgent | ChannelStrategyAgent | Same early strategy loop until repeated independent workload exists |
| StoryContinuityAgent | Quality and Child Safety | Continuity is a quality gate in MVP |
| OriginalityAgent | Quality and Child Safety | Originality is a policy/IP gate in MVP |
| ScriptReviewAgent | Quality and Child Safety | Script review belongs to quality gate initially |
| VoiceDirectorAgent | AI Media Integrator | Voice provider scope is deferred |
| MediaProductionAgent | AI Media Integrator | Provider integrations are adapter work |
| RenderAgent | Media Pipeline | Render is already covered |
| PolicyComplianceAgent | Quality and Child Safety | Existing policy agent covers it |
| PublishingAgent | Publishing and Operations | Existing publishing agent covers it |

## Orchestrator Decision

`ContentFactoryOrchestrator` is needed conceptually but should not be a large prompt file.

Responsibility split:

- Codex: implementation of system tasks.
- n8n: schedules, workflow execution, waiting, callbacks, retries, notifications, approval waits.
- .NET Worker: domain state machine, contracts, idempotency, persistence, budget/policy decisions.
- Agents: specialized intelligence that returns versioned JSON.

## Agent Execution Persistence Strategy

Future table/entity, not implemented now:

`AgentExecution(AgentName, Version, InputHash, OutputJson, Model, Provider, PromptVersion, TokensInput, TokensOutput, EstimatedCost, StartedAt, CompletedAt, CorrelationId)`.

Use this to measure cost, compare prompt versions and replay decisions.
