using Application.Features.Notes.Commands;
using Application.Features.Notes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class NotesController : ApiBaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNote(int id)
    {
        var noteDto = await Mediator.Send(new GetNoteQuery() { Id = id });
        if (noteDto == null)
            return Ok();

        return Ok(noteDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetNoteList()
    {
        var noteDtoList = await Mediator.Send(new GetNoteListQuery());
        if (noteDtoList == null)
            return Ok();

        return Ok(noteDtoList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteCommand command)
    {
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
