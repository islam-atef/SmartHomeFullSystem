using Application.DI;
using Infrastructure.DI;
using Infrastructure.Security.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
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
    .AddAuthentication(options =>
    {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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

// CORS setup
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:4200") // your Angular app's URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// register PhysicalFileProvider for serving static files
builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
);


// Add services to the container.
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Build the app
var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// Enable routing
app.UseRouting();
// Enable static files to serve content from wwwroot
app.UseStaticFiles();
// Enable CORS
app.UseCors(MyAllowSpecificOrigins);
// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();
// Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Map controllers
app.MapControllers();
// launch the application
app.Run();
