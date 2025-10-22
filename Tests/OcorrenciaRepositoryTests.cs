using System;
using AvaEdu.Constants;
using AvaEdu.Repositories;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace AvaEdu.Tests
{
    [TestFixture]
    public class OcorrenciaRepositoryTests
    {
        private XrmFakedContext _ctx;
        private IOrganizationService _service;
        private OcorrenciaRepository _repo;

        [SetUp]
        public void Setup()
        {
            _ctx = new XrmFakedContext();
            _service = _ctx.GetOrganizationService();
            _repo = new OcorrenciaRepository();
        }

        #region Create, Retrieve, Update Tests

        [Test]
        public void Create_Retrieve_Update()
        {
            var entidade = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            entidade[OcorrenciaConstants.FieldCpf] = "11122233344";
            _repo.Create(entidade, _service);
            var rec = _repo.Retrieve(entidade.Id, _service, new ColumnSet(OcorrenciaConstants.FieldCpf));
            Assert.AreEqual("11122233344", rec[OcorrenciaConstants.FieldCpf]);
            rec[OcorrenciaConstants.FieldCpf] = "55566677788";
            _repo.Update(rec, _service);
            var updated = _repo.Retrieve(entidade.Id, _service, new ColumnSet(OcorrenciaConstants.FieldCpf));
            Assert.AreEqual("55566677788", updated[OcorrenciaConstants.FieldCpf]);
        }

        [Test]
        public void Retrieve_SemColumnSet_RetornaTodosAtributos()
        {
            var entidade = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            entidade[OcorrenciaConstants.FieldCpf] = "10101010101";
            entidade[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            _ctx.Initialize(new[] { entidade });

            var retrieved = _repo.Retrieve(entidade.Id, _service);
            Assert.IsTrue(retrieved.Contains(OcorrenciaConstants.FieldCpf));
            Assert.IsTrue(retrieved.Contains(OcorrenciaConstants.FieldAssunto));
        }

        [Test]
        public void Create_RetornaGuidValido()
        {
            var entidade = new Entity(OcorrenciaConstants.EntityLogicalName);
            entidade[OcorrenciaConstants.FieldCpf] = "20202020202";
            var id = _repo.Create(entidade, _service);
            Assert.AreNotEqual(Guid.Empty, id);
        }

        #endregion

        #region ExistsAbertaMesmoCpfTipoAssunto Tests

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_True()
        {
            var tipoId = Guid.NewGuid();
            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr[OcorrenciaConstants.FieldCpf] = "12345678901";
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(10);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _ctx.Initialize(new[] { ocorr });
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("12345678901", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), new OptionSetValue(10), _service);
            Assert.IsTrue(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_IgnoresId()
        {
            var tipoId = Guid.NewGuid();
            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr[OcorrenciaConstants.FieldCpf] = "12345678901";
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(11);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _ctx.Initialize(new[] { ocorr });
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("12345678901", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), new OptionSetValue(11), _service, ocorr.Id);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_StatusFechado()
        {
            var tipoId = Guid.NewGuid();
            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr[OcorrenciaConstants.FieldCpf] = "30303030303";
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(12);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            _ctx.Initialize(new[] { ocorr });
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("30303030303", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), new OptionSetValue(12), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_CpfDiferente()
        {
            var tipoId = Guid.NewGuid();
            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr[OcorrenciaConstants.FieldCpf] = "40404040404";
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(13);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _ctx.Initialize(new[] { ocorr });
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("50505050505", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), new OptionSetValue(13), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_TipoDiferente()
        {
            var tipoId1 = Guid.NewGuid();
            var tipoId2 = Guid.NewGuid();
            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr[OcorrenciaConstants.FieldCpf] = "60606060606";
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId1);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(14);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _ctx.Initialize(new[] { ocorr });
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("60606060606", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId2), new OptionSetValue(14), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_AssuntoDiferente()
        {
            var tipoId = Guid.NewGuid();
            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr[OcorrenciaConstants.FieldCpf] = "70707070707";
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(15);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _ctx.Initialize(new[] { ocorr });
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("70707070707", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), new OptionSetValue(16), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_CpfNull()
        {
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto(null, new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid()), new OptionSetValue(1), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_TipoNull()
        {
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("80808080808", null, new OptionSetValue(1), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_AssuntoNull()
        {
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("90909090909", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid()), null, _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_False_NenhumRegistro()
        {
            var ok = _repo.ExistsAbertaMesmoCpfTipoAssunto("99999999999", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid()), new OptionSetValue(20), _service);
            Assert.IsFalse(ok);
        }

        [Test]
        public void ExistsAbertaMesmoCpfTipoAssunto_True_MultiplasCombinacoes()
        {
            var tipoId1 = Guid.NewGuid();
            var tipoId2 = Guid.NewGuid();

            var ocorr1 = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr1[OcorrenciaConstants.FieldCpf] = "11111111111";
            ocorr1[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId1);
            ocorr1[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(25);
            ocorr1[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            var ocorr2 = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorr2[OcorrenciaConstants.FieldCpf] = "11111111111";
            ocorr2[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId2);
            ocorr2[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(26);
            ocorr2[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            _ctx.Initialize(new[] { ocorr1, ocorr2 });

            var ok1 = _repo.ExistsAbertaMesmoCpfTipoAssunto("11111111111", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId1), new OptionSetValue(25), _service);
            var ok2 = _repo.ExistsAbertaMesmoCpfTipoAssunto("11111111111", new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId2), new OptionSetValue(26), _service);

            Assert.IsTrue(ok1);
            Assert.IsTrue(ok2);
        }

        #endregion

        #region IsFechada Tests

        [Test]
        public void IsFechada_True()
        {
            var e = new Entity(OcorrenciaConstants.EntityLogicalName);
            e[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusFechado);
            Assert.IsTrue(_repo.IsFechada(e));
        }

        [Test]
        public void IsFechada_False_SemStatus()
        {
            var e = new Entity(OcorrenciaConstants.EntityLogicalName);
            Assert.IsFalse(_repo.IsFechada(e));
        }

        [Test]
        public void IsFechada_False_StatusAberto()
        {
            var e = new Entity(OcorrenciaConstants.EntityLogicalName);
            e[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            Assert.IsFalse(_repo.IsFechada(e));
        }

        [Test]
        public void IsFechada_False_StatusAtrasado()
        {
            var e = new Entity(OcorrenciaConstants.EntityLogicalName);
            e[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAtrasado);
            Assert.IsFalse(_repo.IsFechada(e));
        }

        [Test]
        public void IsFechada_False_EntidadeNull()
        {
            Assert.IsFalse(_repo.IsFechada(null));
        }

        [Test]
        public void IsFechada_False_StatusNull()
        {
            var e = new Entity(OcorrenciaConstants.EntityLogicalName);
            e[OcorrenciaConstants.FieldStatus] = null;
            Assert.IsFalse(_repo.IsFechada(e));
        }

        #endregion

        #region RetrievePrazoRespostaHoras Tests

        [Test]
        public void RetrievePrazoRespostaHoras_Value()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 7;
            _ctx.Initialize(new[] { tipo });
            var v = _repo.RetrievePrazoRespostaHoras(new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), _service);
            Assert.AreEqual(7, v);
        }

        [Test]
        public void RetrievePrazoRespostaHoras_Null()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            _ctx.Initialize(new[] { tipo });
            var v = _repo.RetrievePrazoRespostaHoras(new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), _service);
            Assert.IsNull(v);
        }

        [Test]
        public void RetrievePrazoRespostaHoras_Null_TipoRefNull()
        {
            var v = _repo.RetrievePrazoRespostaHoras(null, _service);
            Assert.IsNull(v);
        }

        [Test]
        public void RetrievePrazoRespostaHoras_ValorZero()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 0;
            _ctx.Initialize(new[] { tipo });
            var v = _repo.RetrievePrazoRespostaHoras(new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), _service);
            Assert.AreEqual(0, v);
        }

        [Test]
        public void RetrievePrazoRespostaHoras_ValorNegativo()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = -5;
            _ctx.Initialize(new[] { tipo });
            var v = _repo.RetrievePrazoRespostaHoras(new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), _service);
            Assert.AreEqual(-5, v);
        }

        [Test]
        public void RetrievePrazoRespostaHoras_ValorGrande()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 999999;
            _ctx.Initialize(new[] { tipo });
            var v = _repo.RetrievePrazoRespostaHoras(new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId), _service);
            Assert.AreEqual(999999, v);
        }

        #endregion
    }
}
