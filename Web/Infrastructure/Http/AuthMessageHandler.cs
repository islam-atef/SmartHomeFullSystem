namespace Web.Infrastructure.Http
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using Web.Core.Auth;
    using Web.Core.UserDevice;
    using Web.Services;

    public sealed class AuthMessageHandler : DelegatingHandler
    {
        private readonly ITokenStore _tokenStore;
        private readonly IDeviceIdentityStore _identityStore;
        private readonly IAuthApi _authApi;
        private readonly DeviceManagementService _deviceManagement;
        public AuthMessageHandler(ITokenStore tokeStore, IDeviceIdentityStore identityStore, IAuthApi authApi, DeviceManagementService deviceManagement)
        {
            _tokenStore = tokeStore;
            _identityStore = identityStore;
            _authApi = authApi;
            _deviceManagement = deviceManagement;
        }

        /* this method is called for each HTTP request, and look like interseptor in other languages, and we can modify the request before sending it
         * or the response after receiving it.
         *  1- first we check if the Device-MAC header is present, if not we get it from the store or create a new one and add it to the request,
         *  2- second,we add the Authorization header with the access token to the request,
         *      and if we get a 401 Unauthorized response, we try to refresh the token and retry the request once.
        */
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            // 0- set the Device MACAddress header
            if (!request.Headers.Contains("Device-MAC"))
            {
                // Get the MAC address from the storage.
                var deviceIdentity = await _identityStore.GetAsync();
                if (deviceIdentity is null)
                {
                    // If not found, generate a new one and save it.
                    deviceIdentity = await _deviceManagement.CreateDeviceIdentifierAsync();
                    if (deviceIdentity is null)
                    {
                        // If we still don't have a device identity, return unauthorized.
                        await _tokenStore.ClearAsync();
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                }
                // Add the Device-MAC (device identifier) header to the request.
                request.Headers.Add("Device-MAC", deviceIdentity);
            }
            // 1- get the token if exist and add it to the request for the Authorization header from the store
            var tokens = await _tokenStore.GetAsync();
            if (tokens is not null)
            {
                // add the Authorization header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            }
            // 2- send the request
            var response = await base.SendAsync(request, ct);

            // 3- if unauthorized, try to refresh and retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized && tokens?.RefreshToken is not null)
            {
                // Dispose the previous response to free resources
                response.Dispose();

                // Try to refresh the token
                var refreshed = await _authApi.RefreshAsync(tokens.RefreshToken);
                // if refresh failed, clear the store and return unauthorized
                if (refreshed is null)
                {
                    // Clear the tokens from the store
                    await _tokenStore.ClearAsync();
                    // return unauthorized response
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                // create a token Dto
                var token = new AuthTokens(refreshed.AccessToken, refreshed.RefreshToken, refreshed.ExpiresAtUtc);
                // save the new tokens in all cases (we may get new refresh token or not)
                await _tokenStore.SaveAsync(token);

                // retry the request with the new access token
                var retry = CloneRequest(request);

                // set the new Authorization header
                retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);

                // resend the request
                return await base.SendAsync(retry, ct);
            }
            // return the original response if not unauthorized
            return response;
        }


        private static HttpRequestMessage CloneRequest(HttpRequestMessage original)
        {
            // Create a new request message with the same method and URI
            var clone = new HttpRequestMessage(original.Method, original.RequestUri);

            // Copy content (body)
            if (original.Content != null)
            {
                var ms = new MemoryStream();
                original.Content.CopyToAsync(ms).GetAwaiter().GetResult();
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy content headers
                foreach (var h in original.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }

            // Copy headers
            foreach (var header in original.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            // Copy properties
            clone.Version = original.Version;
            return clone;
        }
    }
}
