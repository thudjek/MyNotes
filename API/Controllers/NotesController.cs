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
    public async Task<IActionResult> GetNoteList([FromQuery] string filter)
    {
        //var noteResponseList = await Mediator.Send(new GetNoteListQuery() { Filter = filter });
        //if (noteResponseList == null)
        //    return Ok();

        var noteResponseList = new List<SharedModels.Responses.Notes.NoteResponse>();
        noteResponseList.Add(new SharedModels.Responses.Notes.NoteResponse() { Title = "Za kupiti", Content = "kruske, jabuke, sljive" });
        noteResponseList.Add(new SharedModels.Responses.Notes.NoteResponse() { Title = "Tasks", Content = "prvi, drugi" });
        noteResponseList.Add(new SharedModels.Responses.Notes.NoteResponse() { Title = "Nekaj", Content = "ocu i necu" });

        if (!string.IsNullOrWhiteSpace(filter))
            noteResponseList = noteResponseList.Where(n => n.Title.ToLower().Contains(filter.ToLower()) || n.Content.ToLower().Contains(filter.ToLower())).ToList();

        await Task.CompletedTask;

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

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        await Mediator.Send(new DeleteNoteCommand() { Id = id });
        return NoContent();
    }

    [HttpPost("restore/{id}")]
    public async Task<IActionResult> RestoreNote(int id)
    {
        await Mediator.Send(new RestoreNoteCommand() { Id = id });
        return NoContent();
    }

    [HttpPost("empty-trash")]
    public async Task<IActionResult> EmptyTrash()
    {
        await Mediator.Send(new EmptyTrashCommand());
        return NoContent();
    }
}
