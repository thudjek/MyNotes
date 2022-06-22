using SharedModels.Enums;

namespace SharedModels.Requests.Permissions;
public class GivePermissionsRequest
{
    public int NoteId { get; set; }
    public List<PermissionToGive> Permissions { get; set; }
}

public class PermissionToGive
{
    public string Email { get; set; }
    public PermissionScope Scope { get; set; }
}