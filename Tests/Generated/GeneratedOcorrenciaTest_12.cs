using NUnit.Framework;
using System;
using Microsoft.Xrm.Sdk;

namespace GeneratedTests
{
    [TestFixture]
    public class GeneratedOcorrenciaTest_12
    {
        [Test]
        public void Test_12()
        {
            // Criterion: Ao criar uma ocorrência com CPF, Tipo e Assunto válidos, o sistema deve preencher automaticamente a **data de criação**.
            var ctx = new AvaEdu.Tests.CrmTestContext(); var service = ctx.Service; var repo = new AvaEdu.Repositories.OcorrenciaRepository(); var ocorrService = new AvaEdu.Services.OcorrenciaService(repo); var ent = ctx.BuildOcorrenciaBasica("12345678901", 1); var pluginCtx = ctx.CreatePluginContext("Create", ent); ocorrService.OnCreate(pluginCtx, service); Assert.IsTrue(ent.Contains(AvaEdu.Constants.OcorrenciaConstants.FieldDataCriacao));
        }
    }
}