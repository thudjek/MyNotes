using Application.Features.Permissions.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class PermissionsController : ApiBaseController
{
    [HttpPost]
    [Route("give")]
    public async Task<IActionResult> Give([FromBody] GivePermissionsCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Revoke(int id)
    {
        await Mediator.Send(new RevokePermissionCommand() { Id = id });
        return NoContent();
    }
}
