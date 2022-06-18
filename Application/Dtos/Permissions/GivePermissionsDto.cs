using Domain.Enums;

namespace Application.Dtos.Permissions;
public class GivePermissionsDto
{
    public string Email { get; set; }
    public PermissionScope Scope { get; set; }
}