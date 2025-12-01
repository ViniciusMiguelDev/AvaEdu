using NUnit.Framework;
using System;
using Microsoft.Xrm.Sdk;

namespace GeneratedTests
{
    [TestFixture]
    public class GeneratedOcorrenciaTest_15
    {
        [Test]
        public void Test_15()
        {
            // Criterion: Se já existir uma ocorrência **aberta** para o mesmo CPF, Tipo e Assunto, o sistema deve **impedir a criação** e lançar uma exceção indicando duplicidade.
            var ctx = new AvaEdu.Tests.CrmTestContext(); var setup = ctx.SetupOcorrenciaDuplicada(); var ocorrService = setup.Service; Assert.Throws<Microsoft.Xrm.Sdk.InvalidPluginExecutionException>(() => ocorrService.OnCreate(setup.PluginContext, setup.OrganizationService));
        }
    }
}