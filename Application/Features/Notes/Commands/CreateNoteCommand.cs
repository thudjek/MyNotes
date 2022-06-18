using Application.Common.Interfaces;
using MediatR;
using Domain.Entities;

namespace Application.Features.Notes.Commands;
public class CreateNoteCommand : IRequest<int>
{
    public string Title { get; set; }
    public string Content { get; set; }
}

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, int>
{
    private readonly IAppDbContext _dbContext;
    public CreateNoteCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note()
        {
            Title = request.Title,
            Content = request.Content
        };

        _dbContext.Notes.Add(note);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}