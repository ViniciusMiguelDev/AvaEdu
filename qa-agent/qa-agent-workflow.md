# QA Agent — Workflow

Fluxo de execução:
1. Extrair critérios: ler PRD/user stories e extrair critérios de aceite.
2. Classificar critérios: mapear tipo (status, schema, campo numérico, performance).
3. Gerar testes NUnit por critério em `tests/Generated/`.
4. Rodar `dotnet test` no projeto `tests/`.
5. Converter TRX -> JSON/HTML e publicar em `reports/qa/`.
6. Se existirem falhas, criar comentário no PR ou issue (integração GitHub opcional).

Modo manual:
- Execute: `node agents/run-agent.js --agent docs/workflows/qa-agent/qa-agent.yaml`

Modo CI:
- Configure `.github/workflows/bmad-qa.yml` (template entregue separadamente).