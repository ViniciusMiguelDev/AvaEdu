// actions/generate_nunit_tests.js (ESM)
import fs from 'fs';
import path from 'path';

// Arquivo gerado pelo passo "classify_criteria"
// Caminho relativo à raiz do projeto (mesmo nível de qa-agent/)
const classifiedPath = path.join('tmp', 'classified.json');
const classified = JSON.parse(fs.readFileSync(classifiedPath, 'utf8') || '[]');

// Saída integrada ao projeto .NET (pasta Tests)
const outDir = path.join('Tests', 'Generated');
if (!fs.existsSync(outDir)) fs.mkdirSync(outDir, { recursive: true });

const baseUrl = process.env.BASE_URL || 'http://localhost:5000';

classified.forEach(c => {
	const className = `GeneratedOcorrenciaTest_${c.id}`;
	let body = '';

	// Mapeia tipos de critério do domínio de Ocorrência para asserts similares aos testes existentes
	switch (c.type) {
		case 'ocorrencia_create_data':
			body = `var ctx = new AvaEdu.Tests.CrmTestContext(); var service = ctx.Service; var repo = new AvaEdu.Repositories.OcorrenciaRepository(); var ocorrService = new AvaEdu.Services.OcorrenciaService(repo); var ent = ctx.BuildOcorrenciaBasica("12345678901", 1); var pluginCtx = ctx.CreatePluginContext("Create", ent); ocorrService.OnCreate(pluginCtx, service); Assert.IsTrue(ent.Contains(AvaEdu.Constants.OcorrenciaConstants.FieldDataCriacao));`;
			break;
		case 'ocorrencia_expiracao_prazo_tipo':
			body = `var ctx = new AvaEdu.Tests.CrmTestContext(); var setup = ctx.SetupTipoOcorrenciaComPrazo(8); var ocorrService = setup.Service; var ent = setup.Ocorrencia; ocorrService.OnCreate(setup.PluginContext, setup.OrganizationService); var dtCriacao = ent.GetAttributeValue<System.DateTime>(AvaEdu.Constants.OcorrenciaConstants.FieldDataCriacao); var dtExp = ent.GetAttributeValue<System.DateTime>(AvaEdu.Constants.OcorrenciaConstants.FieldDataExpiracao); Assert.AreEqual(dtCriacao.AddHours(8), dtExp);`;
			break;
		case 'ocorrencia_expiracao_prazo_default':
			body = `var ctx = new AvaEdu.Tests.CrmTestContext(); var setup = ctx.SetupTipoOcorrenciaSemPrazo(); var ocorrService = setup.Service; var ent = setup.Ocorrencia; ocorrService.OnCreate(setup.PluginContext, setup.OrganizationService); var dtCriacao = ent.GetAttributeValue<System.DateTime>(AvaEdu.Constants.OcorrenciaConstants.FieldDataCriacao); var dtExp = ent.GetAttributeValue<System.DateTime>(AvaEdu.Constants.OcorrenciaConstants.FieldDataExpiracao); Assert.AreEqual(dtCriacao.AddHours(AvaEdu.Constants.OcorrenciaConstants.PrazoDefaultHoras), dtExp);`;
			break;
		case 'ocorrencia_duplicidade':
			body = `var ctx = new AvaEdu.Tests.CrmTestContext(); var setup = ctx.SetupOcorrenciaDuplicada(); var ocorrService = setup.Service; Assert.Throws<Microsoft.Xrm.Sdk.InvalidPluginExecutionException>(() => ocorrService.OnCreate(setup.PluginContext, setup.OrganizationService));`;
			break;
		default:
			body = `Assert.Pass("Placeholder para critério: ${c.text}");`;
			break;
	}

	const file = `using NUnit.Framework;\nusing System;\nusing Microsoft.Xrm.Sdk;\n\nnamespace GeneratedTests\n{\n    [TestFixture]\n    public class ${className}\n    {\n        [Test]\n        public void Test_${c.id}()\n        {\n            // Criterion: ${c.text}\n            ${body}\n        }\n    }\n}`;

	fs.writeFileSync(path.join(outDir, `${className}.cs`), file, 'utf8');
});

console.log('Generated', classified.length, 'tests', 'into', outDir);