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
    internal class OcorrenciaRepositoryFake : IOcorrenciaRepository
    {
        public bool ExistsAbertaMesmoCpfTipoAssuntoRet { get; set; }
        public bool IsFechadaRet { get; set; }
        public int? PrazoHorasRet { get; set; }
        public Entity Retrieve(Guid id, IOrganizationService svc, ColumnSet cols = null) => StoredEntity ?? new Entity(OcorrenciaConstants.EntityLogicalName) { Id = id };
        public void Update(Entity entity, IOrganizationService svc) { StoredEntity = entity; }
        public Guid Create(Entity entity, IOrganizationService svc) { StoredEntity = entity; return entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id; }
        public bool ExistsAbertaMesmoCpfTipoAssunto(string cpf, EntityReference tipoRef, OptionSetValue assuntoOs, IOrganizationService svc, Guid? ignoreId = null) => ExistsAbertaMesmoCpfTipoAssuntoRet;
        public bool IsFechada(Entity entidade) => IsFechadaRet;
        public int? RetrievePrazoRespostaHoras(EntityReference tipoRef, IOrganizationService svc) => PrazoHorasRet;
        public Entity StoredEntity { get; set; }
    }

    [TestFixture]
    public class OcorrenciaServiceTests
    {
        private XrmFakedContext _ctx;
        private IOrganizationService _service;
        private OcorrenciaService _serviceClass;
        private OcorrenciaRepositoryFake _repoFake;

        [SetUp]
        public void Setup()
        {
            _ctx = new XrmFakedContext();
            _service = _ctx.GetOrganizationService();
            _repoFake = new OcorrenciaRepositoryFake();
            _serviceClass = new OcorrenciaService(_repoFake);
        }

        private IPluginExecutionContext BuildContext(string message, object target)
        {
            var pc = _ctx.GetDefaultPluginContext();
            pc.MessageName = message;
            pc.InputParameters.Clear();
            pc.InputParameters["Target"] = target;
            return pc;
        }

        #region OnCreate Tests

        [Test]
        public void OnCreate_AtribuiDataCriacaoEExpiracao_ComPrazoDoTipo()
        {
            _repoFake.PrazoHorasRet = 5;
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            ent[OcorrenciaConstants.FieldCpf] = "12345678901";
            ent[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);
            var ctx = BuildContext("Create", ent);
            _serviceClass.OnCreate(ctx, _service);
            Assert.IsTrue(ent.Contains(OcorrenciaConstants.FieldDataCriacao));
            Assert.IsTrue(ent.Contains(OcorrenciaConstants.FieldDataExpiracao));
            var dt = (DateTime)ent[OcorrenciaConstants.FieldDataCriacao];
            var exp = (DateTime)ent[OcorrenciaConstants.FieldDataExpiracao];
            Assert.AreEqual(dt.AddHours(5), exp);
        }

        [Test]
        public void OnCreate_DetectaDuplicado()
        {
            _repoFake.ExistsAbertaMesmoCpfTipoAssuntoRet = true;
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            ent[OcorrenciaConstants.FieldCpf] = "12345678901";
            ent[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);
            var ctx = BuildContext("Create", ent);
            Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnCreate(ctx, _service));
        }

        [Test]
        public void OnCreate_SemPrazoNoTipo_UsaPrazoDefault()
        {
            _repoFake.PrazoHorasRet = null;
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            ent[OcorrenciaConstants.FieldCpf] = "11111111111";
            ent[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(2);
            var ctx = BuildContext("Create", ent);
            _serviceClass.OnCreate(ctx, _service);
            var dt = (DateTime)ent[OcorrenciaConstants.FieldDataCriacao];
            var exp = (DateTime)ent[OcorrenciaConstants.FieldDataExpiracao];
            Assert.AreEqual(dt.AddHours(OcorrenciaConstants.PrazoDefaultHoras), exp);
        }

        [Test]
        public void OnCreate_SemCpf_NaoVerificaDuplicidade()
        {
            _repoFake.ExistsAbertaMesmoCpfTipoAssuntoRet = true;
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            ent[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(3);
            var ctx = BuildContext("Create", ent);
            Assert.DoesNotThrow(() => _serviceClass.OnCreate(ctx, _service));
        }

        [Test]
        public void OnCreate_SemTipo_NaoVerificaDuplicidade()
        {
            _repoFake.ExistsAbertaMesmoCpfTipoAssuntoRet = true;
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldCpf] = "22222222222";
            ent[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(4);
            var ctx = BuildContext("Create", ent);
            Assert.DoesNotThrow(() => _serviceClass.OnCreate(ctx, _service));
        }

        [Test]
        public void OnCreate_SemAssunto_NaoVerificaDuplicidade()
        {
            _repoFake.ExistsAbertaMesmoCpfTipoAssuntoRet = true;
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldCpf] = "33333333333";
            ent[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            var ctx = BuildContext("Create", ent);
            Assert.DoesNotThrow(() => _serviceClass.OnCreate(ctx, _service));
        }

        [Test]
        public void OnCreate_ComDataCriacaoJaDefinida_NaoSubstitui()
        {
            _repoFake.PrazoHorasRet = 10;
            var dataPredefinida = DateTime.UtcNow.AddDays(-1);
            var ent = new Entity(OcorrenciaConstants.EntityLogicalName);
            ent[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            ent[OcorrenciaConstants.FieldCpf] = "44444444444";
            ent[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            ent[OcorrenciaConstants.FieldDataCriacao] = dataPredefinida;
            var ctx = BuildContext("Create", ent);
            _serviceClass.OnCreate(ctx, _service);
            Assert.AreEqual(dataPredefinida, ent[OcorrenciaConstants.FieldDataCriacao]);
        }

        #endregion

        #region OnUpdate Tests

        [Test]
        public void OnUpdate_Fechada_AtualizaDataConclusaoELanca()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            ctx.PreEntityImages["PreImage"] = preImage;
            Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_Fechada_TentandoAlterarNome_DeveLancarExcecao()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldNome] = "Nome Original";
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldNome] = "Tentando Alterar Nome";
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
            Assert.That(ex.Message, Does.Contain("Nome"));
        }

        [Test]
        public void OnUpdate_Fechada_TentandoAlterarEmail_DeveLancarExcecao()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldEmail] = "original@email.com";
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldEmail] = "novo@email.com";
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
            Assert.That(ex.Message, Does.Contain("Email"));
        }

        [Test]
        public void OnUpdate_Fechada_TentandoAlterarDescricao_DeveLancarExcecao()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldDescricao] = "Descrição Original";
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldDescricao] = "Tentando Alterar Descrição";
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
            Assert.That(ex.Message, Does.Contain("Descrição"));
        }

        [Test]
        public void OnUpdate_Fechada_NomeSemAlteracao_DeveLancarExcecaoGenerica()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldNome] = "Nome Original";
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldNome] = "Nome Original";
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_TipoAlterado_RecalculaExpiracao()
        {
            _repoFake.PrazoHorasRet = 10;
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            preImage[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddHours(-2);
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            _serviceClass.OnUpdate(ctx, _service);
            
            Assert.IsTrue(target.Contains(OcorrenciaConstants.FieldDataExpiracao));
        }

        [Test]
        public void OnUpdate_DetectaDuplicado()
        {
            _repoFake.ExistsAbertaMesmoCpfTipoAssuntoRet = true;
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldCpf] = "123";
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(2);
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };
            target[OcorrenciaConstants.FieldCpf] = "123";
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_SemPreImage_RecuperaDoRepositorio()
        {
            var ocorrenciaId = Guid.NewGuid();
            _repoFake.StoredEntity = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            _repoFake.StoredEntity[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _repoFake.StoredEntity[OcorrenciaConstants.FieldCpf] = "99999999999";
            _repoFake.StoredEntity[OcorrenciaConstants.FieldNome] = "João Silva";
            _repoFake.StoredEntity[OcorrenciaConstants.FieldEmail] = "joao@email.com";
            _repoFake.StoredEntity[OcorrenciaConstants.FieldDescricao] = "Descrição teste";
            _repoFake.StoredEntity[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            _repoFake.StoredEntity[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(10);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = ocorrenciaId };
            target[OcorrenciaConstants.FieldCpf] = "88888888888";

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages.Clear();

            Assert.DoesNotThrow(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_TipoNaoAlterado_NaoRecalculaExpiracao()
        {
            _repoFake.PrazoHorasRet = 10;
            var tipoId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            preImage[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow.AddHours(-2);
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            preImage[OcorrenciaConstants.FieldCpf] = "55555555555";
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(7);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };
            target[OcorrenciaConstants.FieldCpf] = "66666666666";

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;

            _serviceClass.OnUpdate(ctx, _service);

            Assert.IsFalse(target.Contains(OcorrenciaConstants.FieldDataExpiracao));
        }

        [Test]
        public void OnUpdate_StatusFechadoComDataConclusao_NaoAtualizaNovamente()
        {
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldDataConclusao] = DateTime.UtcNow.AddHours(-1);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;

            Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_MudandoParaFechado_DefineDataConclusao()
        {
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            preImage[OcorrenciaConstants.FieldCpf] = "12312312312";
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };
            target[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;

            _serviceClass.OnUpdate(ctx, _service);

            Assert.IsTrue(target.Contains(OcorrenciaConstants.FieldDataConclusao));
        }

        [Test]
        public void OnUpdate_Aberta_AlterandoNome_DevePermitir()
        {
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            preImage[OcorrenciaConstants.FieldNome] = "Nome Original";
            preImage[OcorrenciaConstants.FieldCpf] = "12345678901";
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(3);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };
            target[OcorrenciaConstants.FieldNome] = "Nome Alterado";

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;

            Assert.DoesNotThrow(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_Aberta_AlterandoEmail_DevePermitir()
        {
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            preImage[OcorrenciaConstants.FieldEmail] = "original@email.com";
            preImage[OcorrenciaConstants.FieldCpf] = "12345678901";
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(3);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };
            target[OcorrenciaConstants.FieldEmail] = "novo@email.com";

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;

            Assert.DoesNotThrow(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_Aberta_AlterandoDescricao_DevePermitir()
        {
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            preImage[OcorrenciaConstants.FieldDescricao] = "Descrição Original";
            preImage[OcorrenciaConstants.FieldCpf] = "12345678901";
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(3);

            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preImage.Id };
            target[OcorrenciaConstants.FieldDescricao] = "Descrição Alterada";

            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;

            Assert.DoesNotThrow(() => _serviceClass.OnUpdate(ctx, _service));
        }

        [Test]
        public void OnUpdate_Fechada_TentandoAlterarCpf_DeveLancarExcecao()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldCpf] = "12345678901";
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldCpf] = "98765432100";
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
            Assert.That(ex.Message, Does.Contain("CPF"));
        }

        [Test]
        public void OnUpdate_Fechada_TentandoAlterarTipo_DeveLancarExcecao()
        {
            var preId = Guid.NewGuid();
            var tipoOriginalId = Guid.NewGuid();
            var tipoNovoId = Guid.NewGuid();
            
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoOriginalId);
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoNovoId);
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
            Assert.That(ex.Message, Does.Contain("Tipo"));
        }

        [Test]
        public void OnUpdate_Fechada_TentandoAlterarAssunto_DeveLancarExcecao()
        {
            var preId = Guid.NewGuid();
            var preImage = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            preImage[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            preImage[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);
            
            var target = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = preId };
            target[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(2);
            
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.InputParameters["Target"] = target;
            ctx.PreEntityImages["PreImage"] = preImage;
            
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnUpdate(ctx, _service));
            Assert.That(ex.Message, Does.Contain("Assunto"));
        }

        #endregion

        #region OnDelete Tests

        [Test]
        public void OnDelete_Fechada_Lanca()
        {
            _repoFake.IsFechadaRet = true;
            _repoFake.StoredEntity = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            var targetRef = new EntityReference(OcorrenciaConstants.EntityLogicalName, _repoFake.StoredEntity.Id);
            var ctx = BuildContext("Delete", targetRef);
            Assert.Throws<InvalidPluginExecutionException>(() => _serviceClass.OnDelete(ctx, _service));
        }

        [Test]
        public void OnDelete_NaoFechada_NaoLanca()
        {
            _repoFake.IsFechadaRet = false;
            _repoFake.StoredEntity = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            var targetRef = new EntityReference(OcorrenciaConstants.EntityLogicalName, _repoFake.StoredEntity.Id);
            var ctx = BuildContext("Delete", targetRef);
            Assert.DoesNotThrow(() => _serviceClass.OnDelete(ctx, _service));
        }

        [Test]
        public void OnDelete_TargetNaoEEntity_NaoFazNada()
        {
            var ctx = _ctx.GetDefaultPluginContext();
            ctx.MessageName = "Delete";
            ctx.InputParameters["Target"] = "InvalidTarget";
            Assert.DoesNotThrow(() => _serviceClass.OnDelete(ctx, _service));
        }

        [Test]
        public void OnDelete_EntityReferenceOutraEntidade_NaoFazNada()
        {
            var targetRef = new EntityReference("outra_entidade", Guid.NewGuid());
            var ctx = BuildContext("Delete", targetRef);
            Assert.DoesNotThrow(() => _serviceClass.OnDelete(ctx, _service));
        }

        #endregion
    }
}
