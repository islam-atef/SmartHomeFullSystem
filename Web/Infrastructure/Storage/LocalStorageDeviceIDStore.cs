using Microsoft.JSInterop;
using System.Text.Json;
using Web.Core.Auth;
using Web.Core.UserDevice;

namespace Web.Infrastructure.Storage
{
    public class LocalStorageDeviceIDStore : IDeviceIdentityStore
    {
        private readonly IJSRuntime _js;
        private const string Key = "auth_tokens_v1";
        public LocalStorageDeviceIDStore(IJSRuntime js) => _js = js;

        public async Task<string?> GetAsync()
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", Key);
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<string>(json);
        }

        public Task SaveAsync(string id)
            => _js.InvokeVoidAsync("localStorage.setItem", Key, JsonSerializer.Serialize(id)).AsTask();

        public Task ClearAsync()
            => _js.InvokeVoidAsync("localStorage.removeItem", Key).AsTask();
    }
}
