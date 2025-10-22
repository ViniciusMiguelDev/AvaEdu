using System;
using Microsoft.Xrm.Sdk;
using AvaEdu.Repositories;
using AvaEdu.Services;

namespace AvaEdu
{
    /// <summary>
    /// Plugin executado na exclusão de uma ocorrência.
    /// Impede a exclusão de ocorrências com status fechado.
    /// </summary>
    public class DeletePlugin : PluginBase
    {
        private readonly IOcorrenciaService _service;

        public DeletePlugin() : base(typeof(DeletePlugin))
        {
            _service = new OcorrenciaService(new OcorrenciaRepository());
        }

        /// <summary>
        /// Executa a lógica do plugin de exclusão.
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
                _service.OnDelete(context, service);
            }
            catch (Exception ex)
            {
                tracing.Trace($"[DeletePlugin] {ex}");
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
