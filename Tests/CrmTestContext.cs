using System;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace AvaEdu.Tests
{
    internal interface ICrmTestContext
    {
        IOrganizationService Service { get; }
        void Seed(params Entity[] entities);
        IPluginExecutionContext CreatePluginContext(string messageName, object target, Guid? primaryId = null);
    }

    internal class CrmTestContext : ICrmTestContext
    {
        private readonly XrmFakedContext _inner = new XrmFakedContext();
        public IOrganizationService Service { get; }

        public CrmTestContext()
        {
            Service = _inner.GetOrganizationService();
        }

        public Entity BuildOcorrenciaBasica(string cpf, int assunto)
        {
            var ent = new Entity(AvaEdu.Constants.OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ent[AvaEdu.Constants.OcorrenciaConstants.FieldCpf] = cpf;
            ent[AvaEdu.Constants.OcorrenciaConstants.FieldAssunto] = new OptionSetValue(assunto);
            ent[AvaEdu.Constants.OcorrenciaConstants.FieldTipo] = new EntityReference(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName, Guid.NewGuid());
            return ent;
        }

        public (AvaEdu.Services.OcorrenciaService Service, Entity Ocorrencia, IPluginExecutionContext PluginContext, IOrganizationService OrganizationService) SetupTipoOcorrenciaComPrazo(int prazoHoras)
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[AvaEdu.Constants.OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = prazoHoras;

            var ocorrencia = new Entity(AvaEdu.Constants.OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorrencia[AvaEdu.Constants.OcorrenciaConstants.FieldTipo] = new EntityReference(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[AvaEdu.Constants.OcorrenciaConstants.FieldCpf] = "12345678900";
            ocorrencia[AvaEdu.Constants.OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);

            _inner.Initialize(new[] { tipo });

            var repo = new AvaEdu.Repositories.OcorrenciaRepository();
            var svc = new AvaEdu.Services.OcorrenciaService(repo);
            var pluginCtx = CreatePluginContext("Create", ocorrencia);

            return (svc, ocorrencia, pluginCtx, Service);
        }

        public (AvaEdu.Services.OcorrenciaService Service, Entity Ocorrencia, IPluginExecutionContext PluginContext, IOrganizationService OrganizationService) SetupTipoOcorrenciaSemPrazo()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };

            var ocorrencia = new Entity(AvaEdu.Constants.OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            ocorrencia[AvaEdu.Constants.OcorrenciaConstants.FieldTipo] = new EntityReference(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorrencia[AvaEdu.Constants.OcorrenciaConstants.FieldCpf] = "99999999999";
            ocorrencia[AvaEdu.Constants.OcorrenciaConstants.FieldAssunto] = new OptionSetValue(3);

            _inner.Initialize(new[] { tipo });

            var repo = new AvaEdu.Repositories.OcorrenciaRepository();
            var svc = new AvaEdu.Services.OcorrenciaService(repo);
            var pluginCtx = CreatePluginContext("Create", ocorrencia);

            return (svc, ocorrencia, pluginCtx, Service);
        }

        public (AvaEdu.Services.OcorrenciaService Service, IPluginExecutionContext PluginContext, IOrganizationService OrganizationService) SetupOcorrenciaDuplicada()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[AvaEdu.Constants.OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = 5;

            var existente = new Entity(AvaEdu.Constants.OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            existente[AvaEdu.Constants.OcorrenciaConstants.FieldCpf] = "11111111111";
            existente[AvaEdu.Constants.OcorrenciaConstants.FieldTipo] = new EntityReference(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            existente[AvaEdu.Constants.OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);
            existente[AvaEdu.Constants.OcorrenciaConstants.FieldStatus] = new OptionSetValue(AvaEdu.Constants.OcorrenciaConstants.StatusAberto);

            _inner.Initialize(new Entity[] { tipo, existente });

            var nova = new Entity(AvaEdu.Constants.OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            nova[AvaEdu.Constants.OcorrenciaConstants.FieldCpf] = "11111111111";
            nova[AvaEdu.Constants.OcorrenciaConstants.FieldTipo] = new EntityReference(AvaEdu.Constants.OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            nova[AvaEdu.Constants.OcorrenciaConstants.FieldAssunto] = new OptionSetValue(5);

            var repo = new AvaEdu.Repositories.OcorrenciaRepository();
            var svc = new AvaEdu.Services.OcorrenciaService(repo);
            var pluginCtx = CreatePluginContext("Create", nova);

            return (svc, pluginCtx, Service);
        }

        public void Seed(params Entity[] entities)
        {
            if (entities != null && entities.Length > 0)
            {
                _inner.Initialize(entities);
            }
        }

        public IPluginExecutionContext CreatePluginContext(string messageName, object target, Guid? primaryId = null)
        {
            var ctx = _inner.GetDefaultPluginContext();
            ctx.InputParameters.Clear();
            if (target != null)
            {
                if (target is Entity e)
                {
                    if (primaryId.HasValue && primaryId.Value != Guid.Empty)
                    {
                        e.Id = primaryId.Value;
                    }
                    ctx.InputParameters["Target"] = e;
                    ((XrmFakedPluginExecutionContext)ctx).PrimaryEntityId = e.Id;
                }
                else if (target is EntityReference er)
                {
                    ctx.InputParameters["Target"] = er;
                    ((XrmFakedPluginExecutionContext)ctx).PrimaryEntityId = er.Id;
                }
            }
            ((XrmFakedPluginExecutionContext)ctx).MessageName = messageName;
            return ctx;
        }
    }
}
