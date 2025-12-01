using System;
using Microsoft.Xrm.Sdk;
using AvaEdu.Repositories;
using AvaEdu.Services;

namespace AvaEdu
{
    /// <summary>
    /// Plugin executado na criação de uma ocorrência.
    /// Valida e define datas iniciais, prazo e duplicidade.
    /// </summary>
    public class CreatePlugin : PluginBase
    {
        private readonly IOcorrenciaService _service;

        public CreatePlugin() : base(typeof(CreatePlugin))
        {
            _service = new OcorrenciaService(new OcorrenciaRepository());
        }

        /// <summary>
        /// Executa a lógica do plugin de criação.
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
                _service.OnCreate(context, service);
            }
            catch (Exception ex)
            {
                tracing.Trace($"[CreatePlugin] {ex}");
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}