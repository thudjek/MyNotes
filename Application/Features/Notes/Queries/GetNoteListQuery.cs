using Application.Common.Interfaces;
using Application.Dtos.Notes;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notes.Queries;
public class GetNoteListQuery : IRequest<List<NoteDto>>
{
}

public class GetNoteListQueryHandler : IRequestHandler<GetNoteListQuery, List<NoteDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public GetNoteListQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<List<NoteDto>> Handle(GetNoteListQuery request, CancellationToken cancellationToken)
    {
        var notes = await _dbContext.Notes
            .Include(n => n.Permissions)
            .Where(n => n.CreatedBy == _currentUserService.UserId || 
                        n.Permissions.Any(p => p.Email == _currentUserService.Email && (p.Scope == PermissionScope.Read || p.Scope == PermissionScope.ReadAndWrite)))
            .Select(n => new NoteDto()
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Owned = n.CreatedBy == _currentUserService.UserId
            })
            .ToListAsync(cancellationToken);

        return notes;
    }
}