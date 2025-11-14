using Microsoft.JSInterop;
using System.Text.Json;
using Web.Core.Auth;

namespace Web.Infrastructure.Storage
{
    public sealed class LocalStorageTokenStore : ITokenStore
    {
        private readonly IJSRuntime _js;
        private const string Key = "auth_tokens_v1";
        public LocalStorageTokenStore(IJSRuntime js) => _js = js;

        public async Task<AuthTokens?> GetAsync()
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", Key);
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AuthTokens>(json);
        }

        public Task SaveAsync(AuthTokens tokens)
            => _js.InvokeVoidAsync("localStorage.setItem", Key, JsonSerializer.Serialize(tokens)).AsTask();

        public Task ClearAsync()
            => _js.InvokeVoidAsync("localStorage.removeItem", Key).AsTask();
    }
}
