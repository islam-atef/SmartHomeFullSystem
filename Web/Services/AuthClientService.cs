using Web.Core.Auth;
using Web.Identity;
using Web.Models.AuthModel;

namespace Web.Services
{
    public class AuthClientService
    {
        private readonly IAuthApi _authApi;
        private readonly ITokenStore _tokenStore;
        private readonly AppAuthenticationStateProvider _authStateProvider;
        public AuthClientService(IAuthApi authApi, ITokenStore tokenStore,AppAuthenticationStateProvider authStateProvider)
        {
            _authApi = authApi;
            _tokenStore = tokenStore;
            _authStateProvider = authStateProvider;
        }

        public async Task<LoginResultModel> DoLogin(string? email, string? userName, string password)
        {
            var ErrorMessage = string.Empty;
            // check the input parameters
            if ((String.IsNullOrEmpty(email) || String.IsNullOrEmpty(userName)) && (String.IsNullOrEmpty(password)))
                return new(false) ;
            // create the request DTO
            var request = new LoginRequestDTO
            {
                Email = email,
                Username = userName,
                Password = password
            };
            // process the response
            try
            {
                // call the auth API to login
                var authResponse = await _authApi.LoginAsync(request);
                // all good
                //  store the tokens
                if (!String.IsNullOrEmpty(authResponse.AccessToken) && !String.IsNullOrEmpty(authResponse.RefreshToken))
                {
                    var token = new AuthTokens(authResponse.AccessToken, authResponse.RefreshToken, authResponse.ExpiresAtUtc);
                    await _tokenStore.SaveAsync(token);
                    await _authStateProvider.MarkAuthenticatedAsync(token);
                    return new(true);
                }
                else if (authResponse.OtoQuestionId != null || authResponse.OtoQuestionId != Guid.Empty)
                {
                    // OTO question flow, go to the Device verifing page and send the questionId to it (not implemented yet)
                    return new(false, authResponse.OtoQuestionId);
                }
                else { return new(false, null, "No Received Data"); }
            }
            catch(Exception ex)     
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
                return new(false,null, ErrorMessage);
            }
        }
    }
}
