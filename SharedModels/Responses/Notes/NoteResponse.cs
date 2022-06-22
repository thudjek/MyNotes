namespace SharedModels.Responses.Notes;
public class NoteResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool Owned { get; set; }
}