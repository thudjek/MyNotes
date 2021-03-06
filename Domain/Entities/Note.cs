using Domain.Common;

namespace Domain.Entities;
public class Note : BaseEntity
{
    public Note()
    {
        Permissions = new List<Permission>();
    }
    public string Title { get; set; }
    public string Content { get; set; }
    public List<Permission> Permissions { get; private set; }
}