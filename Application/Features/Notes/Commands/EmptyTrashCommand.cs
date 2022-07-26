using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notes.Commands;
public class EmptyTrashCommand : IRequest
{
}

public class EmptyTrashCommandHandler : IRequestHandler<EmptyTrashCommand>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public EmptyTrashCommandHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(EmptyTrashCommand request, CancellationToken cancellationToken)
    {
        var deletedNotes = await _dbContext.Notes
           .Where(n => n.IsDeleted == true && n.CreatedBy == _currentUserService.UserId)
           .ToListAsync(cancellationToken);

        if (deletedNotes.Count > 0)
        {
            _dbContext.Notes.RemoveRange(deletedNotes);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }  

        return Unit.Value;
    }
}