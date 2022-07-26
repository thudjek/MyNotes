using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notes.Commands;
public class RestoreNoteCommand : IRequest
{
    public int Id { get; set; }
}

public class RestoreNoteCommandHandler : IRequestHandler<RestoreNoteCommand>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public RestoreNoteCommandHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(RestoreNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _dbContext.Notes
            .Where(n => n.Id == request.Id && n.IsDeleted == true && n.CreatedBy == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
            throw new NotFoundException(nameof(Note), request.Id, _currentUserService.UserId, "Error has occurred with selected note");

        note.IsDeleted = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}