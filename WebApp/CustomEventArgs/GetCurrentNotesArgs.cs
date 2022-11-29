namespace WebApp.CustomEventArgs;

public class GetCurrentNotesArgs : EventArgs
{
    public GetCurrentNotesArgs(bool isTrash, string filter)
    {
        IsTrash = isTrash;
        Filter = filter;
    }

    public bool IsTrash { get; set; }
    public string Filter { get; set; }
}
