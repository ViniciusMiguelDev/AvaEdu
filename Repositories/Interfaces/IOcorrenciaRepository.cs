using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace AvaEdu.Repositories
{
    public interface IOcorrenciaRepository
    {
        Entity Retrieve(Guid id, IOrganizationService svc, ColumnSet cols = null);
        void Update(Entity entity, IOrganizationService svc);
        Guid Create(Entity entity, IOrganizationService svc);
        bool ExistsAbertaMesmoCpfTipoAssunto(string cpf, EntityReference tipoRef, OptionSetValue assuntoOs, IOrganizationService svc, Guid? ignoreId = null);
        bool IsFechada(Entity entidade);
        int? RetrievePrazoRespostaHoras(EntityReference tipoRef, IOrganizationService svc);
    }
}
