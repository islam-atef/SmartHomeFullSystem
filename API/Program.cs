using Application.DI;
using Infrastructure.DI;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationService();  // Application layer setup
builder.Services.AddInfrastructureService(builder.Configuration);  // Infra setup (DB, Repos, etc.)

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
