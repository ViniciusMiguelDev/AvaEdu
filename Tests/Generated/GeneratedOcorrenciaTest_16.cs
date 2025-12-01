using NUnit.Framework;
using System;
using Microsoft.Xrm.Sdk;

namespace GeneratedTests
{
    [TestFixture]
    public class GeneratedOcorrenciaTest_16
    {
        [Test]
        public void Test_16()
        {
            // Criterion: Se o CPF **não for informado**, o sistema **não deve** verificar duplicidade.
            var ctx = new AvaEdu.Tests.CrmTestContext(); var setup = ctx.SetupOcorrenciaDuplicada(); var ocorrService = setup.Service; Assert.Throws<Microsoft.Xrm.Sdk.InvalidPluginExecutionException>(() => ocorrService.OnCreate(setup.PluginContext, setup.OrganizationService));
        }
    }
}