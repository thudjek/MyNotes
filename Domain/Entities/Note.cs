using Domain.Common;

namespace Domain.Entities;
public class Note : BaseEntity
{
    public Note()
    {
        Permissions = new List<Permission>();
    }
    public string Content { get; set; }
    public bool IsDeleted { get; set; }
    public List<Permission> Permissions { get; private set; }
}