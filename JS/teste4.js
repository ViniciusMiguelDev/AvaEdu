function onLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    var formType = formContext.ui.getFormType();
    
    var statusField = formContext.getControl("ava_status");
    
    if (statusField) {
        if (formType === 1) {
            statusField.setDisabled(true);
        } else if (formType === 2) {
            statusField.setDisabled(false);
        }
    }
}