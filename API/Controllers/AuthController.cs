using API.ApiDTOs.AuthControllerDTOs.RequestDTOs;
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
        public async Task<IActionResult> Login([FromBody] LoginReqDTO req, CancellationToken ct = default) // [Done ]
        {
            var deviceMAC = Request.Headers["Device-MAC"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-MAC header is missing.");
            var deviceIP = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var loginRequest = new LoginDTO
            {
                Email = req.Email,
                Username = req.Username,
                Password = req.Password,
                DeviceMACAddress = deviceMAC,
                DeviceIP = deviceIP
            };
            var result = await auth.LoginAsync(loginRequest);
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
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO req, CancellationToken ct = default)// [Done ready for testing]
        {
            var deviceMAC = Request.Headers["Device-MAC"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-MAC header is missing.");
            var logout = new LogoutDTO
            {
                RefreshToken = req.RefreshToken,
                UserEmail = req.UserEmail,
                DeviceMAC = deviceMAC
            };
            var result = await auth.LogoutAsync(logout);
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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO req, CancellationToken ct = default) // [Done]
        {
            var request = new RegisterDTO
            {
                Email = req.Email,
                Username = req.Username,
                Password = req.Password,
                DisplayName = req.DisplayName
            };
            var result = await auth.RegisterAsync(request);
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
        public async Task<IActionResult> ActivateAccount([FromBody] AccountActivationRequestDTO accountActivationDTO, CancellationToken ct = default) // [Done]
        {
            var deviceMAC = Request.Headers["Device-MAC"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-MAC header is missing.");
            var deviceIP = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var accountActivation= new AccountActivationDTO
            {
                UserEmail = accountActivationDTO.UserEmail,
                ActivationToken = accountActivationDTO.ActivationToken,
                DeviceMACAddress = deviceMAC,
                DeviceIP = deviceIP
            };

            var result = await auth.AccountActivation(accountActivation);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.GenericResult.ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    Application.GenericResult.ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result);

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


        // Token Management endpoints :
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshTK, CancellationToken ct = default) // [Done]
        {
            var deviceMAC = Request.Headers["Device-MAC"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-MAC header is missing.");
            var result = await auth.RefreshAsync(refreshTK, deviceMAC!);
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
