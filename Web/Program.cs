using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Web.Core.Auth;
using Web.Core.UserDevice;
using Web.Identity;
using Web.Infrastructure.Http;
using Web.Infrastructure.Storage;
using Web.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

// Configure HttpClient with base address from configuration
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001";

#region HttpClient with AuthMessageHandler for all APIs except AuthApi.
builder.Services.AddTransient<AuthMessageHandler>();
// 1- Named HttpClient with AuthMessageHandler
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<AuthMessageHandler>();
// 2- Provide HttpClient instances from the named client
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiClient");
});
#endregion

#region Authentication State Provider
/*
 * This way:
 *      Requests for the base type AuthenticationStateProvider get the AppAuthenticationStateProvider instance.
 *      Requests for the concrete type AppAuthenticationStateProvider also resolve correctly.
 *  This aligns your DI registrations with your service constructor dependencies and resolves the "unable to resolve service" error.
 */
builder.Services.AddScoped<AppAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AppAuthenticationStateProvider>());
#endregion

#region Core-Infrastructure Storage
builder.Services.AddScoped<ITokenStore, LocalStorageTokenStore>();
builder.Services.AddScoped<IDeviceIdentityStore, LocalStorageDeviceIDStore>();
#endregion

#region Core-Infrastructure/Http APIs
// 1- AuthApi registration.
// Depends on special HttpClient without (AuthMessageHandler), and IDeviceIdentityStore
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
// IAuthApi Uses (AuthClient + IDeviceIdentityStore)
builder.Services.AddScoped<IAuthApi>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("AuthClient");
    var deviceStore = sp.GetRequiredService<IDeviceIdentityStore>();

    return new AuthApi(client, deviceStore);
});

// 2- IUserDeviceApi
builder.Services.AddScoped<IUserDeviceApi, UserDeviceApi>();
#endregion

#region regiser other services
builder.Services.AddScoped<AuthClientService>();
builder.Services.AddScoped<DeviceManagementService>();
#endregion

// Add Authorization Core
builder.Services.AddAuthorizationCore();

// Build and run the application
await builder.Build().RunAsync();
