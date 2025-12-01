# História: Criar e gerenciar Ocorrências

Como atendente do CRM
Quero registrar e acompanhar ocorrências de clientes
Para garantir que nenhum atendimento fique sem resposta ou duplicado

## Contexto de negócio

- Uma ocorrência é representada pela entidade `ava_ocorrencia`.
- Os campos principais são: `ava_cpf`, `ava_tipodeocorrencia`, `ava_assuntoexib`, `ava_status`, datas de criação/expiração/conclusão.
- O tipo de ocorrência (`ava_tipodeocorrencia`) pode ter um prazo de resposta configurado em horas (`ava_prazoderesposta`).

## Critérios de aceite — Criação de Ocorrência

1. Ao criar uma ocorrência com CPF, Tipo e Assunto válidos, o sistema deve preencher automaticamente a **data de criação**.
2. Ao criar uma ocorrência com CPF, Tipo e Assunto válidos, o sistema deve calcular a **data de expiração** somando o prazo de resposta (em horas) configurado no tipo de ocorrência à data de criação.
3. Se o tipo de ocorrência **não tiver prazo configurado**, o sistema deve usar o prazo padrão de `24 horas` para calcular a data de expiração.
4. Se já existir uma ocorrência **aberta** para o mesmo CPF, Tipo e Assunto, o sistema deve **impedir a criação** e lançar uma exceção indicando duplicidade.
5. Se o CPF **não for informado**, o sistema **não deve** verificar duplicidade.
6. Se o Tipo **não for informado**, o sistema **não deve** verificar duplicidade.
7. Se o Assunto **não for informado**, o sistema **não deve** verificar duplicidade.

## Critérios de aceite — Atualização de Ocorrência

8. Se a ocorrência estiver com status **Fechado**, qualquer tentativa de alteração em `Nome`, `Email`, `Descrição` ou `CPF` deve ser impedida, lançando exceção.
9. Se a ocorrência estiver com status **Fechado**, qualquer tentativa de alteração de `Tipo` ou `Assunto` deve ser impedida, lançando exceção.
10. Quando o status da ocorrência for alterado para **Fechado** e ainda não houver data de conclusão, o sistema deve preencher automaticamente a **data de conclusão** com a data/hora atual.
11. Quando o Tipo de ocorrência for alterado em uma ocorrência **Aberta**, o sistema deve **recalcular a data de expiração** com base no prazo do novo tipo, mantendo a data de criação original como base do cálculo.

## Critérios de aceite — Exclusão de Ocorrência

12. Se a ocorrência estiver com status **Fechado**, o sistema deve **impedir a exclusão** e lançar exceção apropriada.
