using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;
public class Permission : BaseEntity
{
    public string Email { get; set; }
    public int NoteId { get; set; }
    public Note Note { get; set; }
    public PermissionScope Scope { get; set; }
}