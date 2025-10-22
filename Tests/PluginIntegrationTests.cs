using System;
using AvaEdu.Constants;
using AvaEdu.Repositories;
using AvaEdu.Services;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace AvaEdu.Tests
{
    [TestFixture]
    public class PluginIntegrationTests
    {
        private XrmFakedContext _ctx;
        private IOrganizationService _service;
        private IOcorrenciaService _ocorrenciaService;

        [SetUp]
        public void Setup()
        {
            _ctx = new XrmFakedContext();
            _service = _ctx.GetOrganizationService();
            _ocorrenciaService = new OcorrenciaService(new OcorrenciaRepository());
        }

        private XrmFakedPluginExecutionContext BuildPluginContext(string messageName, object target, Entity preImage = null)
        {
            var pluginContext = _ctx.GetDefaultPluginContext();
            pluginContext.MessageName = messageName;
            pluginContext.InputParameters.Clear();
            pluginContext.InputParameters["Target"] = target;

            if (preImage != null)
            {
                pluginContext.PreEntityImages.Clear();
                pluginContext.PreEntityImages["PreImage"] = preImage;
            }

            return pluginContext;
        }

        #region CreatePlugin Integration Tests

        [Test]
        public void CreatePlugin_Execute_ComDadosValidos_DeveCriarComSucesso()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 8;
            _ctx.Initialize(new[] { tipo });

            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "12345678900";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);

            var pluginContext = BuildPluginContext("Create", ocorrencia);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnCreate(pluginContext, _service));

            Assert.IsTrue(ocorrencia.Contains(OcorrenciaConstants.FieldDataCriacao));
            Assert.IsTrue(ocorrencia.Contains(OcorrenciaConstants.FieldDataExpiracao));
            var dataCriacao = ocorrencia.GetAttributeValue<DateTime>(OcorrenciaConstants.FieldDataCriacao);
            var dataExpiracao = ocorrencia.GetAttributeValue<DateTime>(OcorrenciaConstants.FieldDataExpiracao);
            Assert.AreEqual(dataCriacao.AddHours(8), dataExpiracao);
        }

        [Test]
        public void CreatePlugin_Execute_ComDuplicidade_DeveLancarExcecao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 5;

            var ocorrenciaExistente = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorrenciaExistente[OcorrenciaConstants.FieldCpf] = "11111111111";
            ocorrenciaExistente[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrenciaExistente[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            ocorrenciaExistente[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            _ctx.Initialize(new Entity[] { tipo, ocorrenciaExistente });

            var novaOcorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            novaOcorrencia[OcorrenciaConstants.FieldCpf] = "11111111111";
            novaOcorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            novaOcorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);

            var pluginContext = BuildPluginContext("Create", novaOcorrencia);

            Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnCreate(pluginContext, _service));
        }

        [Test]
        public void CreatePlugin_Execute_SemPrazoNoTipo_UsaPrazoDefault()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            _ctx.Initialize(new[] { tipo });

            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "99999999999";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(3);

            var pluginContext = BuildPluginContext("Create", ocorrencia);

            _ocorrenciaService.OnCreate(pluginContext, _service);

            var dataCriacao = ocorrencia.GetAttributeValue<DateTime>(OcorrenciaConstants.FieldDataCriacao);
            var dataExpiracao = ocorrencia.GetAttributeValue<DateTime>(OcorrenciaConstants.FieldDataExpiracao);
            Assert.AreEqual(dataCriacao.AddHours(OcorrenciaConstants.PrazoDefaultHoras), dataExpiracao);
        }

        #endregion

        #region UpdatePlugin Integration Tests

        [Test]
        public void UpdatePlugin_Execute_AlterandoTipo_DeveRecalcularExpiracao()
        {
            var tipoAntigoId = Guid.NewGuid();
            var tipoAntigo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoAntigoId };
            tipoAntigo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 5;

            var tipoNovoId = Guid.NewGuid();
            var tipoNovo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoNovoId };
            tipoNovo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 12;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoAntigoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "33333333333";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(2);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            var dataCriacao = DateTime.UtcNow.AddHours(-1);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = dataCriacao;

            _ctx.Initialize(new Entity[] { tipoAntigo, tipoNovo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoNovoId);

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnUpdate(pluginContext, _service));

            Assert.IsTrue(target.Contains(OcorrenciaConstants.FieldDataExpiracao));
            var dataExpiracao = target.GetAttributeValue<DateTime>(OcorrenciaConstants.FieldDataExpiracao);
            var dataExpiracaoEsperada = dataCriacao.AddHours(12);
            Assert.AreEqual(dataExpiracaoEsperada, dataExpiracao);
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaFechada_DeveLancarExcecao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "44444444444";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(7);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddDays(-1);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldCpf] = "55555555555";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaFechada_TentandoAlterarNome_DeveLancarExcecao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "44444444444";
            ocorrencia[OcorrenciaConstants.FieldNome] = "Nome Original";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(7);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddDays(-1);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldNome] = "Tentando Alterar";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
            Assert.That(ex.Message, Does.Contain("Nome"));
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaFechada_TentandoAlterarEmail_DeveLancarExcecao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "44444444444";
            ocorrencia[OcorrenciaConstants.FieldEmail] = "original@email.com";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(7);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddDays(-1);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldEmail] = "novo@email.com";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
            Assert.That(ex.Message, Does.Contain("Email"));
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaFechada_TentandoAlterarDescricao_DeveLancarExcecao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "44444444444";
            ocorrencia[OcorrenciaConstants.FieldDescricao] = "Descrição Original";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(7);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddDays(-1);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldDescricao] = "Tentando Alterar";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
            Assert.That(ex.Message, Does.Contain("Descrição"));
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaAberta_AlterandoNome_DevePermitir()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "33333333333";
            ocorrencia[OcorrenciaConstants.FieldNome] = "Nome Original";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddHours(-2);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldNome] = "Nome Alterado";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaAberta_AlterandoEmail_DevePermitir()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "33333333333";
            ocorrencia[OcorrenciaConstants.FieldEmail] = "original@email.com";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddHours(-2);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldEmail] = "novo@email.com";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
        }

        [Test]
        public void UpdatePlugin_Execute_OcorrenciaAberta_AlterandoDescricao_DevePermitir()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 10;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "33333333333";
            ocorrencia[OcorrenciaConstants.FieldDescricao] = "Descrição Original";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddHours(-2);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldDescricao] = "Descrição Alterada";

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
        }

        [Test]
        public void UpdatePlugin_Execute_AlterandoParaDuplicado_DeveLancarExcecao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 6;

            var ocorrencia1Id = Guid.NewGuid();
            var ocorrencia1 = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrencia1Id };
            ocorrencia1[OcorrenciaConstants.FieldCpf] = "77777777777";
            ocorrencia1[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia1[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(10);
            ocorrencia1[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            var ocorrencia2Id = Guid.NewGuid();
            var ocorrencia2 = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrencia2Id };
            ocorrencia2[OcorrenciaConstants.FieldCpf] = "77777777777";
            ocorrencia2[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia2[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(11);
            ocorrencia2[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia1, ocorrencia2 });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrencia2Id };
            target[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(10);

            var pluginContext = BuildPluginContext("Update", target, ocorrencia2);

            Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
        }

        [Test]
        public void UpdatePlugin_Execute_StatusMudandoParaFechado_DeveDefinirDataConclusao()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 15;

            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[OcorrenciaConstants.FieldCpf] = "88888888888";
            ocorrencia[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(15);
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            ocorrencia[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddHours(-5);

            _ctx.Initialize(new Entity[] { tipo, ocorrencia });

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);

            var pluginContext = BuildPluginContext("Update", target, ocorrencia);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnUpdate(pluginContext, _service));
            
            Assert.IsTrue(target.Contains(OcorrenciaConstants.FieldDataConclusao));
            Assert.IsNotNull(target.GetAttributeValue<DateTime?>(OcorrenciaConstants.FieldDataConclusao));
        }

        #endregion

        #region DeletePlugin Integration Tests

        [Test]
        public void DeletePlugin_Execute_OcorrenciaAberta_DevePermitirDelecao()
        {
            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldCpf] = "22222222222";
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            _ctx.Initialize(new[] { ocorrencia });

            var targetRef = new EntityReference(OcorrenciaConstants.EntityLogicalName, ocorrenciaId);

            var pluginContext = BuildPluginContext("Delete", targetRef);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnDelete(pluginContext, _service));
        }

        [Test]
        public void DeletePlugin_Execute_OcorrenciaFechada_DeveLancarExcecao()
        {
            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldCpf] = "66666666666";
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            ocorrencia[OcorrenciaConstants.FieldDataConclusao] = DateTime.UtcNow;

            _ctx.Initialize(new[] { ocorrencia });

            var targetRef = new EntityReference(OcorrenciaConstants.EntityLogicalName, ocorrenciaId);

            var pluginContext = BuildPluginContext("Delete", targetRef);

            Assert.Throws<InvalidPluginExecutionException>(() => _ocorrenciaService.OnDelete(pluginContext, _service));
        }

        [Test]
        public void DeletePlugin_Execute_OcorrenciaAtrasada_DevePermitirDelecao()
        {
            var ocorrenciaId = Guid.NewGuid();
            var ocorrencia = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            ocorrencia[OcorrenciaConstants.FieldCpf] = "10101010101";
            ocorrencia[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAtrasado);

            _ctx.Initialize(new[] { ocorrencia });

            var targetRef = new EntityReference(OcorrenciaConstants.EntityLogicalName, ocorrenciaId);

            var pluginContext = BuildPluginContext("Delete", targetRef);

            Assert.DoesNotThrow(() => _ocorrenciaService.OnDelete(pluginContext, _service));
        }

        #endregion
    }
}
