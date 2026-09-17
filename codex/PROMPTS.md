# Prompt Templates

## Implementacion

```text
Implementa <TASK_ID> usando codex/PROMPTS.md.

Lee en orden:
1. AGENTS.md
2. codex/IMPLEMENTATION_STATUS.md
3. codex/TASK_EXECUTION_ORDER.md
4. codex/ROADMAP.md solo la fila de <TASK_ID> y dependencias directas
5. codex/FIRST_TASK.md si <TASK_ID> coincide
6. codex/AGENT_MATRIX.md solo el agente seleccionado
7. codex/SKILL_STRATEGY.md solo la skill seleccionada

Resuelve automaticamente agente, skill, budget, FilesAllowed, FilesForbidden, Portal capability y validaciones desde ROADMAP/FIRST_TASK.
Usa un agente principal y maximo un revisor.
No leas archivos fuera de FilesAllowed sin justificar.
No ejecutes proveedores pagos, IdP productivo ni Angular Shell salvo que la tarea lo permita explicitamente.
Ejecuta las validaciones de la tarea y actualiza evidencia compacta.
```

## Fix

```text
Corrige <TASK_ID>.
Error:
<ERROR>

Lee solo:
1. codex/IMPLEMENTATION_STATUS.md
2. codex/AGENT_MATRIX.md
3. archivos directamente implicados en el error

Identifica causa raiz, aplica cambio minimo, valida, documenta si cambia una decision.
```

## Review

```text
Revisa <TASK_ID> en <COMMIT>.
No modifiques hasta identificar defectos.

Usa postura de code review:
- findings primero;
- severidad;
- archivo/linea;
- riesgos y pruebas faltantes.
```

## Sprint Close

```text
Valida cierre de Sprint <N>.

Lee:
1. AGENTS.md
2. codex/IMPLEMENTATION_STATUS.md
3. docs/SPRINTS.md
4. docs/integration/portal-gateway.md si aplica

Verifica evidencia: build, tests, Docker, SQL, Portal, riesgos, PRs.
No marques TERMINADO sin comandos y resultados.
```
