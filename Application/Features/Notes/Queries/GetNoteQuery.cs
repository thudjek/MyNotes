using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedModels.Enums;
using SharedModels.Responses.Notes;

namespace Application.Features.Notes.Queries;
public class GetNoteQuery : IRequest<NoteResponse>
{
    public int Id { get; set; }
}

public class GetNoteQueryHandler : IRequestHandler<GetNoteQuery, NoteResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public GetNoteQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<NoteResponse> Handle(GetNoteQuery request, CancellationToken cancellationToken)
    {
        NoteResponse noteResponse = null;

        var note = await _dbContext.Notes
            .Include(n => n.Permissions)
            .Where(n => n.Id == request.Id && (
                        n.CreatedBy == _currentUserService.UserId || 
                        n.Permissions.Any(p => p.Email == _currentUserService.Email && (p.Scope == (int)PermissionScope.Read || p.Scope == (int)PermissionScope.ReadAndWrite))))
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

        if (note != null)
        {
            noteResponse = new NoteResponse()
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                Text = note.Text,
                Owned = note.CreatedBy == _currentUserService.UserId
            };
        }

        return noteResponse;
    }
}