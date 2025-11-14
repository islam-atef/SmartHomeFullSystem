namespace Web.Infrastructure.Http
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using Web.Core.Auth;

    public sealed class AuthMessageHandler : DelegatingHandler
    {
        private readonly ITokenStore _store;
        private readonly IAuthApi _authApi;

        public AuthMessageHandler(ITokenStore store, IAuthApi authApi)
        {
            _store = store;
            _authApi = authApi;
        }

        // this method is called for each HTTP request, and look like interseptor in other languages, and we can modify the request before sending it
        // or the response after receiving it.
        // here we add the Authorization header with the access token to the request,
        // and if we get a 401 Unauthorized response, we try to refresh the token and retry the request once.
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            // 1- get the token if exist and add it to the request for the Authorization header from the store
            var tokens = await _store.GetAsync();
            if (tokens is not null)
            {
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
                    await _store.ClearAsync();
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                // save the new tokens in all cases (we may get new refresh token or not)
                await _store.SaveAsync(refreshed);

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
