# QA Checklist (para o agente)

Antes de executar:
- [ ] PRD/stories atualizados em `docs/sprint-artifacts/`
- [ ] Servidor de teste (staging/local) rodando na URL configurada em qa-context.md
- [ ] Projeto `tests/` criado (`dotnet new nunit -o tests`)
- [ ] `tests/*.csproj` inclui `tests/Generated/**`

Após execução:
- [ ] Verificar `reports/qa/qa-report.json`
- [ ] Se falhas, confirmar issue criada no repositório
- [ ] Revisar tests gerados e ajustar templates se necessário