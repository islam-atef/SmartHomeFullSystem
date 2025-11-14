using Application.DI;
using Infrastructure.DI;
using Infrastructure.Security.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Application layer setup
builder.Services.AddApplicationService();  
// Infra setup (DB, Repos, etc.)
builder.Services.AddInfrastructureService(builder.Configuration);


// JWT Authentication setup
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>()!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(30) // reduce clock skew tolerance (which means that tokens are valid for 30 seconds after expiration)
        };
        // SignalR token from query string, not from header in case of WebSocket connections or signalR requests 
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"];
                var path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/devices"))
                {
                    ctx.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
