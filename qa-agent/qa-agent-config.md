# QA Agent — Configuração

Nome: QA Agent (BMAD)
Propósito: Gerar, executar e reportar testes automatizados (NUnit) a partir do PRD / user stories presentes em `docs/sprint-artifacts/` e `docs/technical/`.

Paths usados:
- PRD / histórias: `docs/sprint-artifacts/`
- Estratégia técnica: `docs/technical/qa-nunit-strategy.md`
- Pasta de testes gerados: `tests/Generated/`
- Relatórios: `reports/qa/`

Entradas:
- PRD.md, user stories, critérios de aceite

Saídas:
- arquivos de teste C# em `tests/Generated/`
- `reports/TestResult.trx`
- `reports/qa-report.json`

Nota: altere `baseUrl` no qa-context.md para apontar para seu ambiente de teste.