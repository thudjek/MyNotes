using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;
public interface IAppDbContext
{
    DbSet<Note> Notes { get; }
    DbSet<Permission> Permissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
