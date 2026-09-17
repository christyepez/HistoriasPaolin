# Token Efficiency

## Required Read Sequence

1. `AGENTS.md`
2. `codex/IMPLEMENTATION_STATUS.md`
3. Exact task text
4. `docs/SPRINTS.md`
5. `docs/integration/portal-gateway.md` when Portal is involved
6. `codex/AGENT_MATRIX.md`
7. Selected primary agent file only
8. Selected skill strategy row only
9. Directly affected source/docs files

## Optional Reads

| Trigger | Read |
|---|---|
| Portal reuse decision missing | `CodexCommonAgents/registry/reusable-portal-apis.md` |
| Risk of duplicate capability | `CodexCommonAgents/registry/do-not-duplicate.md` |
| Architecture ambiguity | `CodexCommonAgents/agents/02-solution-architect-agent.md` |
| Cross-repo integration | `CodexCommonAgents/agents/01-portal-reuse-agent.md` |
| Sprint orchestration | `.codex/agents/orchestrator.md` |

## Context Budgets

| Budget | Max Files | Rule |
|---|---:|---|
| XS | 8 | Single file/doc/config changes |
| S | 15 | One feature slice, no cross-repo runtime |
| M | 25 | Feature plus tests/docs or one external adapter |
| L | 40 | Cross-repo or multi-service integration; justify every extra read |
| XL | PROHIBIDO | Split before implementation |

## Default Forbidden Reads

Do not read by default: `.env`, `bin/`, `obj/`, `node_modules/`, `coverage/`, `TestResults/`, `media/`, `renders/`, `generated/`, `logs/`, `videos/`, `audio/`, `images/`, `temp/`.

Do not read: all agents, all playbooks, all PortalCorporativo, all CodexCommonAgents.

## Execution Rule

One functional task per session, one primary agent, at most one reviewer. If the task needs more than one reviewer or becomes XL, split it.
