using Application.Auth.DTOs;
using Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService auth) : ControllerBase
    {
        // Login and Logout endpoints :
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO req, CancellationToken ct = default)
        {
            var result = await auth.LoginAsync(req);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO req, CancellationToken ct = default)
        {
            var result = await auth.LogoutAsync(req);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.ErrorMessage);
        }



        // Regiser and Activation endpoints :
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO req, CancellationToken ct = default)
        {
            var result = await auth.RegisterAsync(req);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.Conflict => Conflict(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("activate-account")]
        public async Task<IActionResult> ActivateAccount([FromBody] AccountActivationDTO accountActivationDTO, CancellationToken ct = default)
        {
            var result = await auth.AccountActivation(accountActivationDTO);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.ErrorMessage);

        }



        // Password Management endpoints :
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email, CancellationToken ct = default)
        {
            var result = await auth.SendEmailForForgottenPassword(email);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO rpw, CancellationToken ct = default)
        {
            var result = await auth.ResetPassword(rpw);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }




        // Token Management endpoints :
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshTK, CancellationToken ct = default)
        {
            // For device MAC Address, we can get it from request headers or body. Here, assuming it's from body for simplicity.
            string DeviceMAC = Request.Headers["Device-MAC"];
            var result = await auth.RefreshAsync(refreshTK, DeviceMAC!);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }




        // Account Deletion endpoint :
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] string email, CancellationToken ct = default)
        {
            var result = await auth.DeleteAccount(email);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.ErrorMessage);
        }
    }
}
