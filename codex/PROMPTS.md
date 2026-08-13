# Prompt Templates

## Implementacion

```text
Implementa <TASK_ID> usando codex/PROMPTS.md.

Lee en orden:
1. AGENTS.md
2. codex/IMPLEMENTATION_STATUS.md
3. docs/SPRINTS.md
4. codex/AGENT_MATRIX.md
5. codex/SKILL_STRATEGY.md

Selecciona un agente principal y maximo un revisor.
No leas archivos fuera de FilesAllowed de la tarea sin justificar.
Clasifica Portal capability antes de crear componentes transversales.
Ejecuta validaciones aplicables y actualiza evidencia compacta.
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
