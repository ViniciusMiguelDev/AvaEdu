# QA Agent (BMAD) — AvaEdu

Este agente usa o framework BMAD + AgentVibes para gerar e executar testes NUnit para o projeto `AvaEdu` a partir de PRDs e user stories.

## Estrutura de pastas

- `BMAD/` — documentação de processo, templates e PRDs (`docs/sprint-artifacts/`)
- `qa-agent/` — definição do agente e actions em Node
- `Tests/` — projeto de testes NUnit do AvaEdu
  - `Generated/` — testes C# gerados automaticamente pelo agente

## Como funciona o fluxo

1. **parse_prd** (`actions/parse_prd.js`)
   - Lê arquivos MD conforme `prd_glob` configurado em `qa-agent.yaml` (por padrão `BMAD/docs/sprint-artifacts/*.md`).
   - Extrai critérios de aceite e cenários dados/quando/então.

2. **classify_criteria** (`actions/classify_criteria.js`)
   - Classifica cada critério em tipos (status, schema, numeric, performance, etc.).
   - Salva o resultado em `qa-agent/tmp/classified.json`.

3. **generate_nunit_tests** (`actions/generate_nunit_tests.js`)
   - Lê `qa-agent/tmp/classified.json`.
   - Gera arquivos `.cs` na pasta `Tests/Generated/` usando NUnit.
   - Cada critério vira uma classe `GeneratedTest_{id}` dentro do namespace `GeneratedTests`.

4. **run_nunit** (`actions/run_nunit.js`)
   - Executa `dotnet test AvaEdu.csproj` com logger TRX.
   - Usa a pasta `./reports` para gravar `TestResult.trx`.

5. **report_results** (`actions/report_results.js`)
   - Consolida o TRX em um JSON de QA (por padrão `reports/qa/qa-report-*.json`).

## Como rodar o agente manualmente

Pré‑requisitos:
- Node.js instalado
- .NET SDK compatível com o projeto (`net462`) instalado
- Dependências Node já instaladas (`npm install` na raiz do projeto)

Passos:

1. **Atualizar/confirmar PRDs**
   - Coloque/atualize seus PRDs e user stories em `BMAD/docs/sprint-artifacts/`.

2. **Definir a URL base do sistema**
   - Ajuste `baseUrl` em `qa-agent.yaml` ou exporte a variável de ambiente `BASE_URL` (por exemplo `http://localhost:5000`).

3. **Gerar critérios e testes**

   No diretório raiz do projeto (`AvaEdu`):

   ```cmd
   cd qa-agent
   node actions\parse_prd.js
   node actions\classify_criteria.js
   node actions\generate_nunit_tests.js
   ```

   - Verifique que foram criados arquivos `.cs` em `Tests/Generated/`.

4. **Executar NUnit (dotnet test)**

   Ainda na raiz do projeto (`AvaEdu`):

   ```cmd
   cd ..
   dotnet test AvaEdu.csproj --logger "trx;LogFileName=TestResult.trx" --results-directory .\reports
   ```

   ou deixe o agente chamar esse comando automaticamente via `actions/run_nunit.js`:

   ```cmd
   cd qa-agent
   node actions\run_nunit.js
   ```

5. **Gerar relatório de QA**

   ```cmd
   cd qa-agent
   node actions\report_results.js
   ```

   - O relatório consolidado ficará em `reports/qa/`.

## Como criar novos testes NUnit manualmente

1. Crie um novo arquivo `.cs` em `Tests/` ou `Tests/Generated/` (se quiser manter separado o que é gerado).
2. Use o padrão já existente nos testes do projeto, por exemplo:

   ```csharp
   using NUnit.Framework;

   namespace AvaEdu.Tests
   {
       [TestFixture]
       public class MinhaFuncionalidadeTests
       {
           [Test]
           public void Deve_Fazer_Algo()
           {
               // Arrange
               // Act
               // Assert
               Assert.Pass();
           }
       }
   }
   ```

3. Salve o arquivo e execute novamente `dotnet test AvaEdu.csproj`.

## Como estender o agente

- **Novos tipos de critério**: ajuste a lógica em `actions/classify_criteria.js` e o mapeamento para código em `actions/generate_nunit_tests.js`.
- **Outros projetos de teste**: altere o comando em `actions/run_nunit.js` para apontar para outro `.csproj` ou solução.
- **Outros formatos de relatório**: adapte `actions/report_results.js` para exportar HTML, Markdown ou integrar com sua ferramenta de CI.

## Checklist rápido para uso em CI

1. `npm install`
2. `dotnet build AvaEdu.csproj`
3. `node qa-agent\actions\parse_prd.js`
4. `node qa-agent\actions\classify_criteria.js`
5. `node qa-agent\actions\generate_nunit_tests.js`
6. `node qa-agent\actions\run_nunit.js`
7. Publicar arquivos de `reports/` como artefatos da pipeline.
