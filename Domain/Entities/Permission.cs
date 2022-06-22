using Domain.Common;

namespace Domain.Entities;
public class Permission : BaseEntity
{
    public string Email { get; set; }
    public int NoteId { get; set; }
    public Note Note { get; set; }
    public int Scope { get; set; }
}