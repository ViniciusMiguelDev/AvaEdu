using Microsoft.Xrm.Sdk;

namespace AvaEdu.Services
{
    public interface IOcorrenciaService
    {
        void OnCreate(IPluginExecutionContext context, IOrganizationService service);
        void OnUpdate(IPluginExecutionContext context, IOrganizationService service);
        void OnDelete(IPluginExecutionContext context, IOrganizationService service);
    }
}
