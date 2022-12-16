using Application.Common.Interfaces;
using MediatR;
using Domain.Entities;

namespace Application.Features.Notes.Commands;
public class CreateNoteCommand : IRequest
{
    public string Content { get; set; }
}

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand>
{
    private readonly IAppDbContext _dbContext;
    public CreateNoteCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note()
        {
            Title = "My note",
            Content = request.Content,
            Text = string.Empty
        };

        _dbContext.Notes.Add(note);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}