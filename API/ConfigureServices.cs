using API.Services;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API;

public static class ConfigureServices
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddControllers();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthentication(configuration);

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidIssuer = configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
            };
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = configuration["GoogleAuth:ClientId"];
            options.ClientSecret = configuration["GoogleAuth:ClientSecret"];
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        });

        return services;
    }
}
