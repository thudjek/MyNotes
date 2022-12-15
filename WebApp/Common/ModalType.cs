namespace WebApp.Common;

public class ModalType
{
    public ModalType(string title, string body, string okButtonText, string closeButtonText)
    {
        Title = title;
        Body = body;
        OkButtonText = okButtonText;
        CloseButtonText = closeButtonText;
    }

    public string Title { get; set; }
    public string Body { get; set; }
    public string OkButtonText { get; set; }
    public string CloseButtonText { get; set; }

    public static ModalType EmptyTrashModal()
    {
        return new ModalType("Empty Trash", "Are you sure you want to delete everything in trash? This action cannot be reverted.", "Delete", "No");
    }
}
