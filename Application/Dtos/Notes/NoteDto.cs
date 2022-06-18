namespace Application.Dtos.Notes;
public class NoteDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool Owned { get; set; }
}