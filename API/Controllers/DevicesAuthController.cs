using API.ApiDTOs.DeviceAuthControllerDTOs;
using Application.Auth.Interfaces;
using Domain.GenericResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesAuthController(IDeviceManagementService device) : ControllerBase
    {
        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyDevice([FromBody] VerifyDeviceReqDTO req, CancellationToken ct = default) //[Done}
        {
            var deviceMAC = Request.Headers["Device-Mac"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-Mac header is missing.");
            var deviceIP = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await device.VerifyOtpAsync(req.OtpQuestionId, req.OtpAnswer, deviceMAC);
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

        [HttpPost("UpdateBrowserId")]
        public async Task<IActionResult> UpdateDeviceIdentifier([FromBody] string newId, CancellationToken ct = default) //[Done}
        {
            var deviceMAC = Request.Headers["Device-Mac"].ToString();
            if (string.IsNullOrEmpty(deviceMAC))
                return BadRequest("Device-Mac header is missing.");
            var result = await device.UpdateDeviceMACAsync(deviceMAC, newId);
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
    }
}
