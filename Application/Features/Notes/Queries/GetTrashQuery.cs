using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedModels.Responses.Notes;

namespace Application.Features.Notes.Queries;
public class GetTrashQuery : IRequest<List<NoteResponse>>
{

}

public class GetTrashQueryHandler : IRequestHandler<GetTrashQuery, List<NoteResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public GetTrashQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<List<NoteResponse>> Handle(GetTrashQuery request, CancellationToken cancellationToken)
    {
        var noteResponseList = await _dbContext.Notes
            .Include(n => n.Permissions)
            .Where(n => n.CreatedBy == _currentUserService.UserId && n.IsDeleted == true)
            .Select(n => new NoteResponse()
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Text = n.Text,
                Owned = n.CreatedBy == _currentUserService.UserId
            })
            .ToListAsync(cancellationToken);

        return noteResponseList;
    }
}