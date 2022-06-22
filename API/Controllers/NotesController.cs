using Application.Common;
using Application.Features.Notes.Commands;
using Application.Features.Notes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Requests.Notes;

namespace API.Controllers;

[Authorize]
public class NotesController : ApiBaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNote(int id)
    {
        var noteResponse = await Mediator.Send(new GetNoteQuery() { Id = id });
        if (noteResponse == null)
            return Ok();

        return Ok(noteResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetNoteList()
    {
        var noteResponseList = await Mediator.Send(new GetNoteListQuery());
        if (noteResponseList == null)
            return Ok();

        return Ok(noteResponseList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
    {
        var command = Mapper.MapTo<CreateNoteCommand>(request);
        var id = await Mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteRequest request)
    {
        var command = Mapper.MapTo<UpdateNoteCommand>(request);
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        await Mediator.Send(new DeleteNoteCommand() { Id = id });
        return NoContent();
    }
}
