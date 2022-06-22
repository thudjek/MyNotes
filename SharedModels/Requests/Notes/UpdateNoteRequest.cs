namespace SharedModels.Requests.Notes;
public class UpdateNoteRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}