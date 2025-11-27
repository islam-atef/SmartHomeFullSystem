using Web.Core.Auth;

namespace Web.Core.UserDevice
{
    public interface IDeviceIdentityStore
    {
        Task<string?> GetAsync();
        Task SaveAsync(string id);
        Task ClearAsync();
    }
}
