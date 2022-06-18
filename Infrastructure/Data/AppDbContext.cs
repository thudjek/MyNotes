using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data;
public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>, IAppDbContext
{
    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService _currentUserService;
    public AppDbContext(DbContextOptions<AppDbContext> options, IDateTimeService dateTimeService, ICurrentUserService currentUserService) : base(options)
    {
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Permission> Permissions => Set<Permission>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.DateCreated = _dateTimeService.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUserService.UserId;
                    entry.Entity.DateModified = _dateTimeService.Now;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        IdentityConfiguration(builder);
    }

    private static void IdentityConfiguration(ModelBuilder builder)
    {
        builder.Entity<User>().Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Entity<User>().Property(u => u.UserName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
    }
}
