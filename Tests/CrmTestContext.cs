using System;
using System.Collections.Generic;
using AvaEdu.Constants;
using AvaEdu.Repositories;
using AvaEdu.Services;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy;

namespace AvaEdu.Tests
{
    public class CrmTestContext
    {
        private readonly XrmFakedContext _context;
        public IOrganizationService Service => _context.GetOrganizationService();

        public CrmTestContext()
        {
            _context = new XrmFakedContext();
        }

        public Entity BuildOcorrenciaBasica(string cpf, int prazoHoras)
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            tipo[OcorrenciaConstants.FieldTipoPrazoRespostaHoras] = prazoHoras;
            _context.Initialize(new List<Entity> { tipo });

            var ocorr = new Entity(OcorrenciaConstants.EntityLogicalName);
            ocorr[OcorrenciaConstants.FieldCpf] = cpf;
            ocorr[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            ocorr[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);
            ocorr[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            return ocorr;
        }

        public IPluginExecutionContext CreatePluginContext(string messageName, Entity target)
        {
            var pluginContext = _context.GetDefaultPluginContext();
            pluginContext.MessageName = messageName;
            if (!pluginContext.InputParameters.Contains("Target"))
            {
                pluginContext.InputParameters.Add("Target", target);
            }
            else
            {
                pluginContext.InputParameters["Target"] = target;
            }
            return pluginContext;
        }

        public SetupResult SetupOcorrenciaDuplicada()
        {
            var tipoId = Guid.NewGuid();
            var tipo = new Entity(OcorrenciaConstants.TipoEntityLogicalName) { Id = tipoId };
            var existing = new Entity(OcorrenciaConstants.EntityLogicalName) { Id = Guid.NewGuid() };
            existing[OcorrenciaConstants.FieldCpf] = "12345678901";
            existing[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            existing[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);
            existing[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);
            _context.Initialize(new List<Entity> { tipo, existing });

            var newEntity = new Entity(OcorrenciaConstants.EntityLogicalName);
            newEntity[OcorrenciaConstants.FieldCpf] = "12345678901";
            newEntity[OcorrenciaConstants.FieldTipo] = new EntityReference(OcorrenciaConstants.TipoEntityLogicalName, tipoId);
            newEntity[OcorrenciaConstants.FieldAssunto] = new OptionSetValue(1);
            newEntity[OcorrenciaConstants.FieldStatus] = new OptionSetValue(OcorrenciaConstants.StatusAberto);

            var pluginContext = CreatePluginContext("Create", newEntity);
            var repo = new OcorrenciaRepository();
            var ocorrService = new OcorrenciaService(repo);

            return new SetupResult
            {
                Service = ocorrService,
                PluginContext = pluginContext,
                OrganizationService = Service,
                Ocorrencia = newEntity
            };
        }

        public SetupResult SetupTipoOcorrenciaComPrazo(int horas)
        {
            var ocorr = BuildOcorrenciaBasica("12345678901", horas);
            var pluginContext = CreatePluginContext("Create", ocorr);
            var repo = new OcorrenciaRepository();
            var ocorrService = new OcorrenciaService(repo);

            return new SetupResult
            {
                Service = ocorrService,
                PluginContext = pluginContext,
                OrganizationService = Service,
                Ocorrencia = ocorr
            };
        }
    }

    public class SetupResult
    {
        public OcorrenciaService Service { get; set; }
        public IPluginExecutionContext PluginContext { get; set; }
        public IOrganizationService OrganizationService { get; set; }
        public Entity Ocorrencia { get; set; }
    }
}
