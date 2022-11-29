using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Components;

public class CustomValidator : ComponentBase
{
    ValidationMessageStore messageStore;

    [CascadingParameter]
    private EditContext EditContext { get; set; }

    protected override void OnInitialized()
    {
        if (EditContext != null)
        {
            messageStore = new ValidationMessageStore(EditContext);
            EditContext.OnValidationRequested += (s, e) => messageStore.Clear();
            EditContext.OnFieldChanged += (s, e) => messageStore.Clear();
        }
    }

    public void DisplayErrors(string[] errors)
    {
        foreach (var error in errors)
        {
            messageStore.Add(EditContext.Field(""), error);
        }

        EditContext.NotifyValidationStateChanged();
    }

    public void DisplayErrors(Dictionary<string, string[]> errorsGrouped)
    {
        foreach (var error in errorsGrouped)
        {
            messageStore.Add(EditContext.Field(error.Key), error.Value);
        }

        EditContext.NotifyValidationStateChanged();
    }

    public void ClearErrors()
    {
        messageStore?.Clear();
        EditContext?.NotifyValidationStateChanged();
    }
}
