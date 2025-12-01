using NUnit.Framework;
using System;
using Microsoft.Xrm.Sdk;

namespace GeneratedTests
{
    [TestFixture]
    public class GeneratedOcorrenciaTest_14
    {
        [Test]
        public void Test_14()
        {
            // Criterion: Se o tipo de ocorrência **não tiver prazo configurado**, o sistema deve usar o prazo padrão de `24 horas` para calcular a data de expiração.
            var ctx = new AvaEdu.Tests.CrmTestContext(); var setup = ctx.SetupTipoOcorrenciaComPrazo(8); var ocorrService = setup.Service; var ent = setup.Ocorrencia; ocorrService.OnCreate(setup.PluginContext, setup.OrganizationService); var dtCriacao = ent.GetAttributeValue<System.DateTime>(AvaEdu.Constants.OcorrenciaConstants.FieldDataCriacao); var dtExp = ent.GetAttributeValue<System.DateTime>(AvaEdu.Constants.OcorrenciaConstants.FieldDataExpiracao); Assert.AreEqual(dtCriacao.AddHours(8), dtExp);
        }
    }
}