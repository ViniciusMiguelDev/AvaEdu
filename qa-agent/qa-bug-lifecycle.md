# Ciclo de vida de bugs (QA Agent)

1. Agente detecta falha -> cria `reports/qa/issue-suggestion-<timestamp>.md`.
2. Se integrado com GitHub (token presente), agente cria Issue com:
   - Título padronizado: [QA][AUTOMATED] <endpoint> - <erro>
   - Corpo: steps to reproduce (test generated), logs, TRX excerpt
3. Humano revisa a issue -> assinala prioridade -> cria branch de correção.
4. Se habilitado (autofix), agente pode sugerir um PR com branch `fix/qa-<id>` contendo correções triviais.