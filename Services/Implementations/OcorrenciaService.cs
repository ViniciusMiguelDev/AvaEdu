using System;
using AvaEdu.Constants;
using AvaEdu.Repositories;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AvaEdu.Services
{
    public class OcorrenciaService : IOcorrenciaService
    {
        private readonly IOcorrenciaRepository _repo;

        public OcorrenciaService(IOcorrenciaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Executa a lógica de criação de uma ocorrência.
        /// Define a data de criação, calcula a data de expiração baseada no tipo e valida duplicidade.
        /// </summary>
        public void OnCreate(IPluginExecutionContext context, IOrganizationService service)
        {
            var entity = GetTargetEntity(context);
            if (entity == null) return;

            DefinirDataCriacao(entity);
            CalcularDataExpiracao(entity, service);
            ValidarDuplicidade(entity, service);
        }

        /// <summary>
        /// Executa a lógica de atualização de uma ocorrência.
        /// Valida se campos protegidos não foram alterados em ocorrências fechadas.
        /// Recalcula datas quando necessário e valida duplicidade.
        /// </summary>
        public void OnUpdate(IPluginExecutionContext context, IOrganizationService service)
        {
            var target = GetTargetEntity(context);
            if (target == null) return;

            var preImage = ObterPreImage(context, target, service);
            if (preImage == null) return;

            var statusAnterior = preImage.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldStatus)?.Value;
            
            if (statusAnterior == OcorrenciaConstants.StatusFechado)
            {
                ValidarAlteracaoEmOcorrenciaFechada(target, preImage);
            }

            AtualizarDataConclusao(target, preImage);
            RecalcularDataExpiracao(target, preImage, service);
            ValidarDuplicidadeNaAtualizacao(target, preImage, service);
        }

        /// <summary>
        /// Executa a lógica de exclusão de uma ocorrência.
        /// Impede a exclusão de ocorrências já fechadas.
        /// </summary>
        public void OnDelete(IPluginExecutionContext context, IOrganizationService service)
        {
            var entityReference = GetTargetEntityReference(context);
            if (entityReference == null) return;

            var entidade = _repo.Retrieve(entityReference.Id, service, new ColumnSet(OcorrenciaConstants.FieldStatus));
            
            if (_repo.IsFechada(entidade))
            {
                throw new InvalidPluginExecutionException("Ocorrência fechada não pode ser apagada.");
            }
        }

        #region Métodos Auxiliares

        /// <summary>
        /// Extrai a entidade Target do contexto de execução.
        /// </summary>
        private Entity GetTargetEntity(IPluginExecutionContext context)
        {
            if (context.InputParameters.Contains("Target") && 
                context.InputParameters["Target"] is Entity entity && 
                entity.LogicalName == OcorrenciaConstants.EntityLogicalName)
            {
                return entity;
            }
            return null;
        }

        /// <summary>
        /// Extrai a referência Target do contexto de execução.
        /// </summary>
        private EntityReference GetTargetEntityReference(IPluginExecutionContext context)
        {
            if (context.InputParameters.Contains("Target") && 
                context.InputParameters["Target"] is EntityReference entityRef && 
                entityRef.LogicalName == OcorrenciaConstants.EntityLogicalName)
            {
                return entityRef;
            }
            return null;
        }

        /// <summary>
        /// Define a data de criação se ainda não foi definida.
        /// </summary>
        private void DefinirDataCriacao(Entity entity)
        {
            if (!entity.Contains(OcorrenciaConstants.FieldDataCriacao))
            {
                entity[OcorrenciaConstants.FieldDataCriacao] = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Calcula e define a data de expiração baseada no prazo do tipo de ocorrência.
        /// </summary>
        private void CalcularDataExpiracao(Entity entity, IOrganizationService service)
        {
            var tipo = entity.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo);
            var prazoHoras = _repo.RetrievePrazoRespostaHoras(tipo, service) ?? OcorrenciaConstants.PrazoDefaultHoras;
            var agora = DateTime.UtcNow;
            
            entity[OcorrenciaConstants.FieldDataExpiracao] = agora.AddHours(prazoHoras);
        }

        /// <summary>
        /// Valida se já existe uma ocorrência aberta com mesmo CPF, Tipo e Assunto.
        /// </summary>
        private void ValidarDuplicidade(Entity entity, IOrganizationService service)
        {
            var cpf = entity.Contains(OcorrenciaConstants.FieldCpf) 
                ? entity[OcorrenciaConstants.FieldCpf]?.ToString() 
                : null;
            var tipo = entity.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo);
            var assunto = entity.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldAssunto);

            if (!string.IsNullOrWhiteSpace(cpf) && tipo != null && assunto != null)
            {
                if (_repo.ExistsAbertaMesmoCpfTipoAssunto(cpf, tipo, assunto, service))
                {
                    throw new InvalidPluginExecutionException(
                        "Já existe uma ocorrência em aberto para este CPF, Tipo e Assunto.");
                }
            }
        }

        /// <summary>
        /// Obtém a PreImage do contexto ou busca do banco de dados se necessário.
        /// </summary>
        private Entity ObterPreImage(IPluginExecutionContext context, Entity target, IOrganizationService service)
        {
            Entity preImage = null;

            if (context.PreEntityImages != null && context.PreEntityImages.Contains("PreImage"))
            {
                preImage = context.PreEntityImages["PreImage"];
            }

            if (preImage == null && target.Id != Guid.Empty)
            {
                preImage = _repo.Retrieve(target.Id, service, new ColumnSet(
                    OcorrenciaConstants.FieldStatus,
                    OcorrenciaConstants.FieldDataConclusao,
                    OcorrenciaConstants.FieldDataCriacao,
                    OcorrenciaConstants.FieldTipo,
                    OcorrenciaConstants.FieldCpf,
                    OcorrenciaConstants.FieldNome,
                    OcorrenciaConstants.FieldEmail,
                    OcorrenciaConstants.FieldDescricao,
                    OcorrenciaConstants.FieldAssunto));
            }

            return preImage;
        }

        /// <summary>
        /// Valida se campos protegidos foram alterados em uma ocorrência fechada.
        /// Lança exceção se houver tentativa de alteração.
        /// </summary>
        private void ValidarAlteracaoEmOcorrenciaFechada(Entity target, Entity preImage)
        {
            ValidarCampoTextoNaoAlterado(target, preImage, OcorrenciaConstants.FieldNome, "Nome");
            ValidarCampoTextoNaoAlterado(target, preImage, OcorrenciaConstants.FieldEmail, "Email");
            ValidarCampoTextoNaoAlterado(target, preImage, OcorrenciaConstants.FieldDescricao, "Descrição");
            ValidarCampoTextoNaoAlterado(target, preImage, OcorrenciaConstants.FieldCpf, "CPF");
            ValidarCampoReferenciaOuOptionSetNaoAlterado(target, preImage);

            throw new InvalidPluginExecutionException("Ocorrência fechada não pode ser editada.");
        }

        /// <summary>
        /// Valida se um campo de texto foi alterado.
        /// </summary>
        private void ValidarCampoTextoNaoAlterado(Entity target, Entity preImage, string fieldName, string fieldLabel)
        {
            if (target.Contains(fieldName))
            {
                var valorOriginal = preImage.GetAttributeValue<string>(fieldName) ?? string.Empty;
                var valorNovo = target.GetAttributeValue<string>(fieldName) ?? string.Empty;
                
                if (!string.Equals(valorOriginal, valorNovo, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidPluginExecutionException(
                        $"Ocorrência fechada não pode ter o {fieldLabel} alterado.");
                }
            }
        }

        /// <summary>
        /// Valida se campos de referência (Tipo) ou OptionSet (Assunto) foram alterados.
        /// </summary>
        private void ValidarCampoReferenciaOuOptionSetNaoAlterado(Entity target, Entity preImage)
        {
            if (target.Contains(OcorrenciaConstants.FieldTipo))
            {
                var tipoOriginal = preImage.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo);
                var tipoNovo = target.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo);
                
                if (tipoOriginal?.Id != tipoNovo?.Id)
                {
                    throw new InvalidPluginExecutionException("Ocorrência fechada não pode ter o Tipo alterado.");
                }
            }

            if (target.Contains(OcorrenciaConstants.FieldAssunto))
            {
                var assuntoOriginal = preImage.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldAssunto);
                var assuntoNovo = target.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldAssunto);
                
                if (assuntoOriginal?.Value != assuntoNovo?.Value)
                {
                    throw new InvalidPluginExecutionException("Ocorrência fechada não pode ter o Assunto alterado.");
                }
            }
        }

        /// <summary>
        /// Atualiza a data de conclusão quando o status é alterado para fechado.
        /// </summary>
        private void AtualizarDataConclusao(Entity target, Entity preImage)
        {
            if (target.Contains(OcorrenciaConstants.FieldStatus))
            {
                var novoStatus = target.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldStatus)?.Value;
                var possuiDataConclusao = preImage.Contains(OcorrenciaConstants.FieldDataConclusao);
                
                if (novoStatus == OcorrenciaConstants.StatusFechado && !possuiDataConclusao)
                {
                    target[OcorrenciaConstants.FieldDataConclusao] = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Recalcula a data de expiração se o tipo foi alterado.
        /// </summary>
        private void RecalcularDataExpiracao(Entity target, Entity preImage, IOrganizationService service)
        {
            if (target.Contains(OcorrenciaConstants.FieldTipo))
            {
                var tipoNovo = target.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo) 
                    ?? preImage.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo);
                
                var prazoHoras = _repo.RetrievePrazoRespostaHoras(tipoNovo, service) 
                    ?? OcorrenciaConstants.PrazoDefaultHoras;
                
                var dataCriacao = preImage.GetAttributeValue<DateTime?>(OcorrenciaConstants.FieldDataCriacao) 
                    ?? DateTime.UtcNow;
                
                target[OcorrenciaConstants.FieldDataExpiracao] = dataCriacao.AddHours(prazoHoras);
            }
        }

        /// <summary>
        /// Valida duplicidade considerando os valores atuais ou da PreImage.
        /// </summary>
        private void ValidarDuplicidadeNaAtualizacao(Entity target, Entity preImage, IOrganizationService service)
        {
            var cpf = target.Contains(OcorrenciaConstants.FieldCpf)
                ? target[OcorrenciaConstants.FieldCpf]?.ToString()
                : preImage.GetAttributeValue<string>(OcorrenciaConstants.FieldCpf);

            var tipo = target.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo)
                ?? preImage.GetAttributeValue<EntityReference>(OcorrenciaConstants.FieldTipo);

            var assunto = target.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldAssunto)
                ?? preImage.GetAttributeValue<OptionSetValue>(OcorrenciaConstants.FieldAssunto);

            if (!string.IsNullOrWhiteSpace(cpf) && tipo != null && assunto != null)
            {
                if (_repo.ExistsAbertaMesmoCpfTipoAssunto(cpf, tipo, assunto, service, preImage.Id))
                {
                    throw new InvalidPluginExecutionException(
                        "Já existe outra ocorrência em aberto para este CPF, Tipo e Assunto.");
                }
            }
        }

        #endregion
    }
}
