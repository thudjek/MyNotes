using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedModels.Enums;
using SharedModels.Responses.Notes;

namespace Application.Features.Notes.Queries;
public class GetNoteListQuery : IRequest<List<NoteResponse>>
{
}

public class GetNoteListQueryHandler : IRequestHandler<GetNoteListQuery, List<NoteResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public GetNoteListQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<List<NoteResponse>> Handle(GetNoteListQuery request, CancellationToken cancellationToken)
    {
        var noteResponseList = await _dbContext.Notes
            .Include(n => n.Permissions)
            .Where(n => n.CreatedBy == _currentUserService.UserId ||
                        (n.Permissions.Any(p => p.Email == _currentUserService.Email && 
                                         (p.Scope == (int)PermissionScope.Read || p.Scope == (int)PermissionScope.ReadAndWrite)) && 
                        n.IsDeleted == false))
            .Select(n => new NoteResponse()
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Text= n.Text,
                Owned = n.CreatedBy == _currentUserService.UserId,
                IsDeleted = n.IsDeleted,
                Date = n.DateCreated
            })
            .ToListAsync(cancellationToken);

        return noteResponseList;
    }
}