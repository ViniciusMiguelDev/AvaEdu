# QA Context (variáveis e configuração)

BaseUrl: http://localhost:5000   # <- altere para sua API/staging
TestsOutput: tests/Generated
PrdPaths:
  - docs/sprint-artifacts/PRD.md
  - docs/sprint-artifacts/
ReportsPath: reports/qa
Github:
  enabled: false
  repo: your-org/your-repo
  tokenEnvVar: GITHUB_TOKEN