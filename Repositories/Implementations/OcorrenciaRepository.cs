using AvaEdu.Constants;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace AvaEdu.Repositories
{
    public class OcorrenciaRepository : IOcorrenciaRepository
    {
        /// <summary>
        /// Cria uma nova entidade no CRM.
        /// </summary>
        /// <param name="entity">Entidade a ser criada</param>
        /// <param name="svc">Serviço de organização</param>
        /// <returns>ID da entidade criada</returns>
        public Guid Create(Entity entity, IOrganizationService svc)
        {
            return svc.Create(entity);
        }

        /// <summary>
        /// Recupera uma entidade do CRM pelo ID.
        /// </summary>
        /// <param name="id">ID da entidade</param>
        /// <param name="svc">Serviço de organização</param>
        /// <param name="cols">Colunas a serem recuperadas (padrão: todas)</param>
        /// <returns>Entidade recuperada</returns>
        public Entity Retrieve(Guid id, IOrganizationService svc, ColumnSet cols = null)
        {
            return svc.Retrieve(
                OcorrenciaConstants.EntityLogicalName, 
                id, 
                cols ?? new ColumnSet(true));
        }

        /// <summary>
        /// Atualiza uma entidade no CRM.
        /// </summary>
        /// <param name="entity">Entidade com os dados atualizados</param>
        /// <param name="svc">Serviço de organização</param>
        public void Update(Entity entity, IOrganizationService svc)
        {
            svc.Update(entity);
        }

        /// <summary>
        /// Verifica se existe uma ocorrência aberta com o mesmo CPF, Tipo e Assunto.
        /// </summary>
        /// <param name="cpf">CPF a ser verificado</param>
        /// <param name="tipoRef">Referência do tipo de ocorrência</param>
        /// <param name="assuntoOs">Valor do assunto</param>
        /// <param name="svc">Serviço de organização</param>
        /// <param name="ignoreId">ID a ser ignorado na busca (para atualização)</param>
        /// <returns>True se existir ocorrência duplicada</returns>
        public bool ExistsAbertaMesmoCpfTipoAssunto(
            string cpf, 
            EntityReference tipoRef, 
            OptionSetValue assuntoOs, 
            IOrganizationService svc, 
            Guid? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(cpf) || tipoRef == null || assuntoOs == null)
                return false;

            var query = ConstruirQueryDuplicidade(cpf, tipoRef, assuntoOs, ignoreId);
            var resultado = svc.RetrieveMultiple(query);
            
            return resultado.Entities.Count > 0;
        }

        /// <summary>
        /// Verifica se a ocorrência está fechada.
        /// </summary>
        /// <param name="entidade">Entidade a ser verificada</param>
        /// <returns>True se a ocorrência estiver fechada</returns>
        public bool IsFechada(Entity entidade)
        {
            if (entidade == null || !entidade.Contains(OcorrenciaConstants.FieldStatus))
                return false;

            var status = entidade.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldStatus);
            return status != null && status.Value == OcorrenciaConstants.StatusFechado;
        }

        /// <summary>
        /// Recupera o prazo de resposta em horas configurado no tipo de ocorrência.
        /// </summary>
        /// <param name="tipoRef">Referência do tipo de ocorrência</param>
        /// <param name="svc">Serviço de organização</param>
        /// <returns>Prazo em horas ou null se não configurado</returns>
        public int? RetrievePrazoRespostaHoras(EntityReference tipoRef, IOrganizationService svc)
        {
            if (tipoRef == null) 
                return null;

            var tipo = svc.Retrieve(
                OcorrenciaConstants.TipoEntityLogicalName, 
                tipoRef.Id, 
                new ColumnSet(OcorrenciaConstants.FieldTipoPrazoRespostaHoras));

            if (tipo.Contains(OcorrenciaConstants.FieldTipoPrazoRespostaHoras))
            {
                return tipo.GetAttributeValue<int?>(OcorrenciaConstants.FieldTipoPrazoRespostaHoras);
            }

            return null;
        }

        #region Métodos Auxiliares Privados

        /// <summary>
        /// Constrói a query para verificar duplicidade de ocorrências.
        /// </summary>
        private QueryExpression ConstruirQueryDuplicidade(
            string cpf, 
            EntityReference tipoRef, 
            OptionSetValue assuntoOs, 
            Guid? ignoreId)
        {
            var query = new QueryExpression(OcorrenciaConstants.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1
            };

            query.Criteria.AddCondition(OcorrenciaConstants.FieldCpf, ConditionOperator.Equal, cpf);
            query.Criteria.AddCondition(OcorrenciaConstants.FieldTipo, ConditionOperator.Equal, tipoRef.Id);
            query.Criteria.AddCondition(OcorrenciaConstants.FieldAssunto, ConditionOperator.Equal, assuntoOs.Value);
            query.Criteria.AddCondition(OcorrenciaConstants.FieldStatus, ConditionOperator.Equal, OcorrenciaConstants.StatusAberto);

            if (ignoreId.HasValue)
            {
                query.Criteria.AddCondition(OcorrenciaConstants.EntityPrimaryId, ConditionOperator.NotEqual, ignoreId.Value);
            }

            return query;
        }

        #endregion
    }
}
