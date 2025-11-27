using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Web.Core.Auth;
using Web.Core.Utile;

namespace Web.Identity
{
    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStore _store;
        private readonly IAuthApi _authApi;
        private readonly TimeProvider _clock;
        private static readonly TimeSpan RefreshSkew = TimeSpan.FromMinutes(2);
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public AppAuthenticationStateProvider(ITokenStore store, IAuthApi authApi, TimeProvider clock)
        {
            _store = store;
            _authApi = authApi;
            _clock = clock;
        }


        // Get the current authentication state and build the claims for the current user
        // and it is responsable for the Authentication state.
        // refreshing tokens if needed.
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anon = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var tokens = await _store.GetAsync();
            if (tokens is null) return anon;

            // Check if we need to refresh
            var now = _clock.GetUtcNow().UtcDateTime;
            var needRefresh = tokens.ExpiresAtUtc <= now + RefreshSkew;

            if (needRefresh && !string.IsNullOrWhiteSpace(tokens.RefreshToken))
            {
                // single-flight refresh
                await _refreshLock.WaitAsync();
                try
                {
                    // re-read in case it changed meanwhile
                    var latest = await _store.GetAsync();
                    // return unauthinticated if the store is empty.
                    if (latest is null) return anon;
                    // if there is a token in the store:
                    if (latest.ExpiresAtUtc <= _clock.GetUtcNow().UtcDateTime + RefreshSkew)
                    {
                        var refreshed = await _authApi.RefreshAsync(latest.RefreshToken);
                        if (refreshed is null || String.IsNullOrEmpty(refreshed.AccessToken) || String.IsNullOrEmpty(refreshed.RefreshToken))
                        {
                            await _store.ClearAsync();
                            NotifyAuthenticationStateChanged(Task.FromResult(anon));
                            return anon;
                        }
                        tokens = new AuthTokens(refreshed.AccessToken,refreshed.RefreshToken,refreshed.ExpiresAtUtc);
                        await _store.SaveAsync(tokens);
                    }
                    else
                    {
                        // another tab may have refreshed already
                        tokens = latest; 
                    }
                }
                finally { _refreshLock.Release(); }
            }
            var identity = JwtClaimReader.BuildIdentityFromJwt(tokens.AccessToken);
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);
            return state;
        }

        // Mark the user as authenticated and notify the authentication state has changed
        public async Task MarkAuthenticatedAsync(AuthTokens tokens)
        {
            await _store.SaveAsync(tokens);
            var identity = JwtClaimReader.BuildIdentityFromJwt(tokens.AccessToken);
            var state = new AuthenticationState(new ClaimsPrincipal(identity));
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public async Task LogoutAsync()
        {
            await _store.ClearAsync();
            var anon = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(anon));
        }
    }
}

