using API.ApiDTOs.AuthControllerDTOs.RequestDTOs;
using Application.Auth.DTOs;
using Application.Auth.Interfaces;
using Domain.GenericResult;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

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
            var deviceMAC = Request.Headers["Device-Mac"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-Mac header is missing.");
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
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                    _ => BadRequest(result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO req, CancellationToken ct = default) // [Done]
        {
            var deviceMAC = Request.Headers["Device-Mac"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-Mac header is missing.");
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
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
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
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.Conflict => Conflict(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("activate-account")]
        public async Task<IActionResult> ActivateAccount([FromBody] AccountActivationRequestDTO accountActivationDTO, CancellationToken ct = default) // [Done]
        {
            var deviceMAC = Request.Headers["Device-Mac"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-Mac header is missing.");
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
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result);

        }



        // Password Management endpoints :
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDTO reqDto, CancellationToken ct = default) // [Done]
        {
            var email = reqDto.Email;
            var result = await auth.SendEmailForForgottenPassword(email);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO rpw, CancellationToken ct = default) // [Done]
        {
            var result = await auth.ResetPassword(rpw);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }



        // Token Management endpoints :
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDTO refreshDto, CancellationToken ct = default) // [Done]
        {
            var deviceMAC = Request.Headers["Device-Mac"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-Mac header is missing.");
            var result = await auth.RefreshAsync(refreshDto.refreshTK, deviceMAC!);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.Value);
        }



        // Check Existance endpoints :
        [HttpGet("check-email")]
        public async Task<ActionResult<GenericResult<bool>>> CheckEmail([FromQuery] string email) // [Done]
        {
            var result = await auth.CheckEmailExistanceAsync(email);
            if (!result.IsSuccess && result.ErrorType == ErrorType.NullableValue)
                return BadRequest(result.ErrorMessage);
            if (!result.IsSuccess && result.ErrorType == ErrorType.Conflict)
                return StatusCode(StatusCodes.Status409Conflict, result.ErrorMessage); 
            return Ok(result.Value);
        }
  
        [HttpGet("check-username")]
        public async Task<ActionResult<GenericResult<bool>>> CheckUsername([FromQuery] string username) // [Done]
        {
            var result = await auth.CheckUserNameExistanceAsync(username);
            if (!result.IsSuccess && result.ErrorType == ErrorType.NullableValue)
                return BadRequest(result.ErrorMessage);
            if (!result.IsSuccess && result.ErrorType == ErrorType.Conflict)
                return StatusCode(StatusCodes.Status409Conflict, result.ErrorMessage);
            return Ok(result.Value);
        }



        // Account Deletion endpoint :
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteRequestDTO emailDto, CancellationToken ct = default)
        {
            var result = await auth.DeleteAccount(emailDto.email);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.InvalidData => BadRequest(result.ErrorMessage),
                    ErrorType.NotFound => NotFound(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }
            return Ok(result.ErrorMessage);
        }
    }
}
