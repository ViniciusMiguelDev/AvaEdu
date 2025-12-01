// actions/classify_criteria.js (ESM)
import fs from 'fs';

const criteria = JSON.parse(fs.readFileSync('tmp/criteria.json', 'utf8') || '[]');

// Mapeia frases de critérios em tipos de teste ligados ao domínio de Ocorrência
function classify(c) {
	const lower = c.toLowerCase();

	if (lower.includes('cria') && lower.includes('data de cria')) return 'ocorrencia_create_data';
	if (lower.includes('data de expira') && lower.includes('prazo')) return 'ocorrencia_expiracao_prazo_tipo';
	if (lower.includes('prazo padrão') || lower.includes('24 horas')) return 'ocorrencia_expiracao_prazo_default';
	if (lower.includes('duplicidade') || lower.includes('já existir uma ocorrência aberta')) return 'ocorrencia_duplicidade';
	if (lower.includes('não deve verificar duplicidade') && lower.includes('cpf')) return 'ocorrencia_sem_duplicidade_sem_cpf';
	if (lower.includes('não deve verificar duplicidade') && lower.includes('tipo')) return 'ocorrencia_sem_duplicidade_sem_tipo';
	if (lower.includes('não deve verificar duplicidade') && lower.includes('assunto')) return 'ocorrencia_sem_duplicidade_sem_assunto';
	if (lower.includes('fechad') && lower.includes('impedir a alteração') && (lower.includes('nome') || lower.includes('email') || lower.includes('descri') || lower.includes('cpf'))) return 'ocorrencia_fechar_campos_texto';
	if (lower.includes('fechad') && lower.includes('impedir a alteração') && (lower.includes('tipo') || lower.includes('assunto'))) return 'ocorrencia_fechar_tipo_assunto';
	if (lower.includes('status') && lower.includes('fechado') && lower.includes('data de conclusão')) return 'ocorrencia_data_conclusao';
	if (lower.includes('alterado') && lower.includes('tipo') && lower.includes('recalcular a data de expira')) return 'ocorrencia_recalcula_expiracao_tipo';
	if (lower.includes('exclus') && lower.includes('fechad')) return 'ocorrencia_exclusao_fechada';

	return 'generic';
}

const classified = criteria.map((c, i) => ({ id: i, text: c, type: classify(c) }));

fs.writeFileSync('tmp/classified.json', JSON.stringify(classified, null, 2));
console.log('Classified', classified.length, 'criteria');