using Application.Common;
using Application.Features.Permissions.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Requests.Permissions;

namespace API.Controllers;

[Authorize]
public class PermissionsController : ApiBaseController
{
    [HttpPost]
    [Route("give")]
    public async Task<IActionResult> Give([FromBody] GivePermissionsRequest request)
    {
        var command = Mapper.MapTo<GivePermissionsCommand>(request);
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
