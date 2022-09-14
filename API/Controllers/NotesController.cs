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
        //var noteResponseList = await Mediator.Send(new GetNoteListQuery());
        //if (noteResponseList == null)
        //    return Ok();

        var noteResponseList = new List<SharedModels.Responses.Notes.NoteResponse>
        {
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Za kupiti", Content = "kruske, jabuke, sljive", Date = new DateTime(2022, 2, 5) },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Tasks", Content = "prvi, drugi", Date = new DateTime(2022, 2, 3) },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Nekaj", Content = "ocu i necu", Date = new DateTime(2022, 2, 8) },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Za kupiti izbrisano", Content = "kruske, jabuke, sljive, banane", IsDeleted = true, Date = new DateTime(2022, 2, 1) },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Tasks 222", Content = "prvi, drugi, treci, cetvrti", IsDeleted = true, Date = new DateTime(2022, 2, 16) },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Nekaj 223", Content = "ocu i necu i mozda", IsDeleted = true, Date = new DateTime(2022, 2, 10) }
        };

        await Task.CompletedTask;

        return Ok(noteResponseList);
    }

    [HttpGet("trash")]
    public async Task<IActionResult> GetTrashNoteList()
    {
        //var noteResponseList = await Mediator.Send(new GetNoteListQuery());
        //if (noteResponseList == null)
        //    return Ok();

        var noteResponseList = new List<SharedModels.Responses.Notes.NoteResponse>
        {
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Za kupiti izbrisano", Content = "kruske, jabuke, sljive, banane" },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Tasks 222", Content = "prvi, drugi, treci, cetvrti" },
            new SharedModels.Responses.Notes.NoteResponse() { Title = "Nekaj 223", Content = "ocu i necu i mozda" }
        };

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
