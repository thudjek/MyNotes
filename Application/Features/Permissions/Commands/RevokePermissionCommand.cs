using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Permissions.Commands;
public class RevokePermissionCommand : IRequest
{
    public int Id { get; set; }
}

public class RevokePermissionCommandHandler : IRequestHandler<RevokePermissionCommand>
{
    private readonly IAppDbContext _appDbContext;
    private readonly ICurrentUserService _currentUserService;
    public RevokePermissionCommandHandler(IAppDbContext appDbContext, ICurrentUserService currentUserService)
    {
        _appDbContext = appDbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(RevokePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _appDbContext.Permissions
            .Where(p => p.Id == request.Id && p.CreatedBy == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (permission == null)
            throw new NotFoundException(nameof(Permission), request.Id, _currentUserService.UserId, "You cannot revoke permission you didn't give");

        _appDbContext.Permissions.Remove(permission);

        await _appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}