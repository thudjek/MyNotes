using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;
public class DatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    public DatabaseInitializer(ILogger<DatabaseInitializer> logger, AppDbContext dbContext, UserManager<User> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task InitializeDatabase()
    {
        try
        {
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await _dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while initializig database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while seeding database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        var user = new User()
        {
            Email = "test@test.com",
            UserName = "test@test.com",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != user.Email))
        {
            await _userManager.CreateAsync(user, "ASDqwe123");
        }
    }
}