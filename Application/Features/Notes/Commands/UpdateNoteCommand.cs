using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notes.Commands;
public class UpdateNoteCommand : IRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Text { get; set; }
}

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public UpdateNoteCommandHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _dbContext.Notes
            .Where(n => n.Id == request.Id && n.CreatedBy == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null || note.CreatedBy != _currentUserService.UserId)
            throw new NotFoundException(nameof(Note), request.Id, _currentUserService.UserId, "Error has occurred with selected note");

        note.Title = request.Title ?? string.Empty;
        note.Content = request.Content;
        note.Text = request.Text ?? string.Empty;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}