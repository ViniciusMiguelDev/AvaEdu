/**
 * Valida um CPF conforme as regras brasileiras de validação.
 * Remove caracteres não numéricos e verifica os dígitos verificadores.
 * @param {string} cpf - CPF a ser validado
 * @returns {boolean} True se o CPF for válido
 */
function validarCPF(cpf) {
    cpf = cpf.replace(/[^\d]/g, '');
    
    if (!possuiTamanhoValido(cpf) || possuiDigitosRepetidos(cpf)) {
        return false;
    }
    
    return validarDigitosVerificadores(cpf);
}

/**
 * Verifica se o CPF possui 11 dígitos.
 * @param {string} cpf - CPF limpo (apenas números)
 * @returns {boolean} True se possuir 11 dígitos
 */
function possuiTamanhoValido(cpf) {
    return cpf.length === 11;
}

/**
 * Verifica se todos os dígitos do CPF são iguais.
 * @param {string} cpf - CPF limpo (apenas números)
 * @returns {boolean} True se todos os dígitos forem iguais
 */
function possuiDigitosRepetidos(cpf) {
    return /^(\d)\1{10}$/.test(cpf);
}

/**
 * Valida os dois dígitos verificadores do CPF.
 * @param {string} cpf - CPF limpo (apenas números)
 * @returns {boolean} True se os dígitos verificadores forem válidos
 */
function validarDigitosVerificadores(cpf) {
    var primeiroDigito = calcularDigitoVerificador(cpf, 9, 10);
    if (primeiroDigito !== parseInt(cpf[9])) {
        return false;
    }
    
    var segundoDigito = calcularDigitoVerificador(cpf, 10, 11);
    return segundoDigito === parseInt(cpf[10]);
}

/**
 * Calcula um dígito verificador do CPF.
 * @param {string} cpf - CPF limpo (apenas números)
 * @param {number} quantidade - Quantidade de dígitos a processar
 * @param {number} multiplicadorInicial - Multiplicador inicial para o cálculo
 * @returns {number} Dígito verificador calculado
 */
function calcularDigitoVerificador(cpf, quantidade, multiplicadorInicial) {
    var soma = 0;
    for (var i = 0; i < quantidade; i++) {
        soma += parseInt(cpf[i]) * (multiplicadorInicial - i);
    }
    return (11 - (soma % 11)) % 11;
}

/**
 * Evento disparado ao alterar o campo CPF.
 * Valida o CPF e exibe notificação se inválido.
 * @param {object} executionContext - Contexto de execução
 */
function aoMudarCampo(executionContext) {
    var formContext = executionContext.getFormContext();
    var cpf = formContext.getAttribute('ava_cpf').getValue();
    var controle = formContext.getControl('ava_cpf');
    
    if (cpf && !validarCPF(cpf)) {
        controle.setNotification('CPF inválido.', 'ava_cpf_erro');
    } else {
        controle.clearNotification('ava_cpf_erro');
    }
}

/**
 * Evento disparado ao salvar o formulário.
 * Impede o salvamento se o CPF for inválido.
 * @param {object} executionContext - Contexto de execução
 */
function aoSalvar(executionContext) {
    var formContext = executionContext.getFormContext();
    var cpf = formContext.getAttribute('ava_cpf').getValue();
    
    if (cpf && !validarCPF(cpf)) {
        formContext.getControl('ava_cpf').setNotification('CPF inválido.', 'ava_cpf_erro');
        executionContext.getEventArgs().preventDefault();
    }
}