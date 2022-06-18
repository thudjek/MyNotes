using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notes.Commands;
public class DeleteNoteCommand : IRequest
{
    public int Id { get; set; }
}

public class MediatRClass1Handler : IRequestHandler<DeleteNoteCommand>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public MediatRClass1Handler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _dbContext.Notes
            .Where(n => n.Id == request.Id && n.CreatedBy == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
            throw new NotFoundException(nameof(Note), request.Id, _currentUserService.UserId);

        _dbContext.Notes.Remove(note);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}