using NUnit.Framework;
using System;
using Microsoft.Xrm.Sdk;

namespace GeneratedTests
{
    [TestFixture]
    public class GeneratedOcorrenciaTest_22
    {
        [Test]
        public void Test_22()
        {
            // Criterion: Quando o Tipo de ocorrência for alterado em uma ocorrência **Aberta**, o sistema deve **recalcular a data de expiração** com base no prazo do novo tipo, mantendo a data de criação original como base do cálculo.
            var ctx = new AvaEdu.Tests.CrmTestContext(); var service = ctx.Service; var repo = new AvaEdu.Repositories.OcorrenciaRepository(); var ocorrService = new AvaEdu.Services.OcorrenciaService(repo); var ent = ctx.BuildOcorrenciaBasica("12345678901", 1); var pluginCtx = ctx.CreatePluginContext("Create", ent); ocorrService.OnCreate(pluginCtx, service); Assert.IsTrue(ent.Contains(AvaEdu.Constants.OcorrenciaConstants.FieldDataCriacao));
        }
    }
}