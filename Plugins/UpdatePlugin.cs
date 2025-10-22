using System;
using Microsoft.Xrm.Sdk;
using AvaEdu.Repositories;
using AvaEdu.Services;

namespace AvaEdu
{
    /// <summary>
    /// Plugin executado na atualização de uma ocorrência.
    /// Valida alterações em ocorrências fechadas e recalcula datas quando necessário.
    /// </summary>
    public class UpdatePlugin : PluginBase
    {
        private readonly IOcorrenciaService _service;

        public UpdatePlugin() : base(typeof(UpdatePlugin))
        {
            _service = new OcorrenciaService(new OcorrenciaRepository());
        }

        /// <summary>
        /// Executa a lógica do plugin de atualização.
        /// </summary>
        protected override void ExecuteDataversePlugin(ILocalPluginContext localContext)
        {
            var serviceProvider = localContext.ServiceProvider;
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                _service.OnUpdate(context, service);
            }
            catch (Exception ex)
            {
                tracing.Trace($"[UpdatePlugin] {ex}");
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
