using API.ApiDTOs.DeviceAuthControllerDTOs;
using Application.Auth.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesAuthController(IDeviceManagementService device) : ControllerBase
    {
        [HttpPost("VerifyDevice")]
        public async Task<IActionResult> VerifyDevice([FromBody] VerifyDeviceReqDTO req, CancellationToken ct = default) //[Done}
        {
            var deviceMAC = Request.Headers["Device-MAC"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-MAC header is missing.");
            var deviceIP = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await device.VerifyOtpAsync(req.OtoQuestionId, req.OtpAnswer, deviceMAC);
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

        [HttpPost("UpdateDeviceIdentifier")]
        public async Task<IActionResult> UpdateDeviceIdentifier([FromBody] string newMAC, CancellationToken ct = default) //[Done}
        {
            var deviceMAC = Request.Headers["Device-MAC"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-MAC header is missing.");
            var result = await device.UpdateDeviceMACAsync(deviceMAC, newMAC);
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
    }
}
