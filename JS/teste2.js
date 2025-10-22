function atualizarVisibilidadeCampos(executionContext) {
    var formContext = executionContext.getFormContext();

    var status = formContext.getAttribute("ava_status").getValue();

    formContext.getControl("ava_datadecriacao").setVisible(false);
    formContext.getControl("ava_datadeexpiracao").setVisible(false);
    formContext.getControl("ava_datadeconclusao").setVisible(false);

    if (status === "Aberto") {
        formContext.getControl("ava_datadecriacao").setVisible(true);
        formContext.getControl("ava_datadeexpiracao").setVisible(true);
    } 
    else if (status === "Fechado") {
        formContext.getControl("ava_datadecriacao").setVisible(true);
        formContext.getControl("ava_datadeconclusao").setVisible(true);
    } 
    else if (status === "Atrasado") {
        formContext.getControl("ava_datadecriacao").setVisible(true);
        formContext.getControl("ava_datadeexpiracao").setVisible(true);
    }
}
