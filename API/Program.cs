using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Serilog.Events;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    ConfigureServices(builder.Services, builder.Configuration);

    var app = builder.Build();

    ConfigureMiddleware(app, app.Services);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start");
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuiration) 
{
    services.AddApplicationServices();
    services.AddInfrastructureServices(configuiration);

    services.AddControllers();
    services.AddRouting(options => options.LowercaseUrls = true);
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    var iss = configuiration["JWT:ValidIssuer"];
    var aud = configuiration["JWT:ValidAudience"];
    var key = configuiration["JWT:Secret"];

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
            ValidAudience = configuiration["JWT:ValidAudience"],
            ValidIssuer = configuiration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuiration["JWT:Secret"]))
        };
    })
    .AddCookie()
    .AddGoogle(options => 
    {
        options.ClientId = configuiration["GoogleAuth:ClientId"];
        options.ClientSecret = configuiration["GoogleAuth:ClientSecret"];
    });

    services.Configure<CookiePolicyOptions>(options => 
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    });
}

static void ConfigureMiddleware(WebApplication app, IServiceProvider services) 
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHsts();

        app.Use((context, next) => 
        {
            context.Request.Host = new HostString(app.Configuration["AppDomain"]);
            context.Request.Scheme = "https";
            return next();
        });
    }


    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();


    app.UseExceptionHandling();

    app.UseAuthentication();

    app.UseAuthorization();
    
    app.MapControllers();
}
