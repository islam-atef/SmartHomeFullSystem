using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Web.Core.Auth;
using Web.Identity;
using Web.Infrastructure.Http;
using Web.Infrastructure.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Core/Infrastructure/Auth
builder.Services.AddScoped<ITokenStore, LocalStorageTokenStore>();
builder.Services.AddScoped<IAuthApi, AuthApi>();

builder.Services.AddScoped<AuthenticationStateProvider,AppAuthenticationStateProvider>();

// HttpClient with AuthMessageHandler
builder.Services.AddTransient<AuthMessageHandler>();


var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
