# Estratégia de Testes NUnit — QA Agent

Tipos de testes gerados automaticamente:
- Status code (200/4xx/5xx)
- Assert de presença de campos (items array)
- Tipos de campo (price numeric)
- Performance básica (tempo de resposta < X ms)
- Integração básica: endpoints encadeados

Estrutura de cada teste gerado:
- Namespace: GeneratedTests
- Classe: GeneratedTest_{n}
- Método: Test_{n} (async)
- Usa HttpClient com BaseAddress configurável via `qa-context.md`

Observações:
- Para testes complexos (auth, DB seed), crie fixtures em `tests/Fixtures/` e adapte templates.