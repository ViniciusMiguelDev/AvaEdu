/**
 * Atualiza as opções do campo Assunto baseado no Tipo de Ocorrência selecionado.
 * Limpa as opções existentes e adiciona apenas as opções válidas para o tipo escolhido.
 * @param {object} executionContext - Contexto de execução do formulário
 */
function atualizarAssunto(executionContext) {
  var formContext = executionContext.getFormContext();
  var tipo = formContext.getAttribute("ava_tipodeocorrencia").getValue();
  var controleAssunto = formContext.getControl("ava_assuntoexib");

  if (!tipo || tipo.length === 0) {
    controleAssunto.clearOptions();
    return;
  }

  var tipoName = tipo[0].name;
  var opcoesPermitidas = obterOpcoesPorTipo(tipoName);

  controleAssunto.clearOptions();
  adicionarOpcoes(controleAssunto, opcoesPermitidas);
  selecionarPrimeiraOpcao(formContext, opcoesPermitidas);
}

/**
 * Retorna as opções de assunto disponíveis para um determinado tipo.
 * @param {string} tipoName - Nome do tipo de ocorrência
 * @returns {Array} Array de objetos com text e value
 */
function obterOpcoesPorTipo(tipoName) {
  var mapeamentoOpcoes = {
    "Financeiro": [
      { text: "Acordos", value: 1 },
      { text: "Cadeiras Adicionais", value: 3 },
      { text: "Mensalidade", value: 6 },
      { text: "Rematrícula", value: 7 },
      { text: "Outros", value: 0 }
    ],
    "Academico": [
      { text: "Grade Curricular", value: 5 },
      { text: "Troca de Curso", value: 8 },
      { text: "Outros", value: 0 }
    ],
    "Tecnico": [
      { text: "Aulas Remotas", value: 2 },
      { text: "Ecossistema", value: 4 },
      { text: "Outros", value: 0 }
    ]
  };

  return mapeamentoOpcoes[tipoName] || [];
}

/**
 * Adiciona opções ao controle de assunto.
 * @param {object} controle - Controle do campo
 * @param {Array} opcoes - Array de opções a serem adicionadas
 */
function adicionarOpcoes(controle, opcoes) {
  opcoes.forEach(function (opcao) {
    controle.addOption(opcao);
  });
}

/**
 * Seleciona automaticamente a primeira opção disponível.
 * @param {object} formContext - Contexto do formulário
 * @param {Array} opcoes - Array de opções disponíveis
 */
function selecionarPrimeiraOpcao(formContext, opcoes) {
  if (opcoes.length > 0) {
    formContext
      .getAttribute("ava_assuntoexib")
      .setValue(opcoes[0].value);
  }
}
