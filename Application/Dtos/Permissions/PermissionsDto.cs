using Domain.Enums;

namespace Application.Dtos.Permissions;
public class PermissionsDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public PermissionScope Scope { get; set; }
}