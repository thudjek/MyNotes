using Application.Common.Interfaces;
using Application.Dtos.Notes;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notes.Queries;
public class GetNoteQuery : IRequest<NoteDto>
{
    public int Id { get; set; }
}

public class GetNoteQueryHandler : IRequestHandler<GetNoteQuery, NoteDto>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public GetNoteQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<NoteDto> Handle(GetNoteQuery request, CancellationToken cancellationToken)
    {
        NoteDto noteDto = null;

        var note = await _dbContext.Notes
            .Include(n => n.Permissions)
            .Where(n => n.Id == request.Id && (
                        n.CreatedBy == _currentUserService.UserId || 
                        n.Permissions.Any(p => p.Email == _currentUserService.Email && (p.Scope == PermissionScope.Read || p.Scope == PermissionScope.ReadAndWrite))))
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

        if (note != null)
        {
            noteDto = new NoteDto()
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                Owned = note.CreatedBy == _currentUserService.UserId
            };
        }

        return noteDto;
    }
}