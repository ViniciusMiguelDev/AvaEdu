using System;

namespace AvaEdu.Constants
{
    public static class OcorrenciaConstants
    {
        public const string EntityLogicalName = "ava_ocorrencia";
        public const string EntityPrimaryId = "ava_ocorrenciaid";
        public const string FieldNome = "ava_nome";
        public const string FieldEmail = "ava_email";
        public const string FieldDescricao = "ava_descricao";
        public const string FieldCpf = "ava_cpf";
        public const string FieldTipo = "ava_tipodeocorrencia";
        public const string FieldAssunto = "ava_assuntoexib";
        public const string FieldStatus = "ava_status";
        public const string FieldState = "statecode";
        public const string FieldDataExpiracao = "ava_datadeexpiracao";
        public const string FieldDataConclusao = "ava_datadeconclusao";
        public const string FieldDataCriacao = "ava_datadecriacao";
        public const string TipoEntityLogicalName = "ava_tipodeocorrencia";
        public const string FieldTipoPrazoRespostaHoras = "ava_prazoderesposta";
        public const int StatusAberto = 751960000;
        public const int StatusFechado = 751960002;
        public const int StatusAtrasado = 751960003;
        public const int PrazoDefaultHoras = 24;
    }
}
