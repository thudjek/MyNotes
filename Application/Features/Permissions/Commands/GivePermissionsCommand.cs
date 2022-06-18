using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Dtos.Permissions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Permissions.Commands;
public class GivePermissionsCommand : IRequest
{
    public int NoteId { get; set; }
    public List<GivePermissionsDto> Permissions { get; set; }

}

public class GivePermissionsCommandHandler : IRequestHandler<GivePermissionsCommand>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    public GivePermissionsCommandHandler(IAppDbContext dbContext, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(GivePermissionsCommand request, CancellationToken cancellationToken)
    {
        var note = await _dbContext.Notes
                    .Include(n => n.Permissions)
                    .Where(n => n.Id == request.NoteId && n.CreatedBy == _currentUserService.UserId)
                    .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
            throw new NotFoundException(nameof(Note), request.NoteId, _currentUserService.UserId);

        request.Permissions.ForEach(p =>
        {
            var permission = note.Permissions.FirstOrDefault(np => np.Email == p.Email);

            if (permission != null)
            {
                permission.Scope = p.Scope;
            }
            else
            {
                note.Permissions.Add(new Permission()
                {
                    Email = p.Email,
                    Scope = p.Scope
                });
            }
        });

        return Unit.Value;
    }
}