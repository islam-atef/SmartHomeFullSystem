using Application.User_Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserInfoController(IUserInfoService infoService) : ControllerBase
    {
        [HttpGet("Get-Info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.GetUserInfoAsync(userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpGet("Get-Homes")]
        public async Task<IActionResult> GetUserHomes()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.GetUserHomesAsync(userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpGet("Get-All-HSRQ")]
        public async Task<IActionResult> GetUserAllSRequest()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.GetUserAllHomeSubscriptionRequsetsAsync(userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpGet("Get-New-HSRQ")]
        public async Task<IActionResult> GetUserNewSRequest()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.GetUserNewHomeSubscriptionRequsetsAsync(userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpPatch("Update-PhoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumeber([FromBody] string phoneNumber)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.ChangeUserPhoneNumberAsync(phoneNumber, userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpPatch("Update-DisplayName")]
        public async Task<IActionResult> UpdateDiplayName([FromBody] string diplayName)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.ChangeUserDisplayNameAsync(diplayName, userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpPatch("Update-UserImage")]
        public async Task<IActionResult> UpdateImage([FromBody] IFormFile image)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.ChangeUserImageAsync(image, userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpPost("Subscribe-ToHome")]
        public async Task<IActionResult> SubscripeToHome([FromBody] string homeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (string.IsNullOrEmpty(homeId))
                return BadRequest($" No Id provided: {homeId}");
            Guid.TryParse(homeId, out var guidHomeId);

            var result = await infoService.SendHomesubscriptionRequestAsync(userId,guidHomeId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpDelete("Delete-SubRequest/{requestId}")]
        public async Task<IActionResult> DeleteSubRequest(string requestId) 
        {
            if (string.IsNullOrEmpty(requestId))
                return BadRequest($" No Id provided: {requestId}");

            if(!Guid.TryParse(requestId, out var guidRequestId))
                return BadRequest("Invalid RequestId Data");

            var result = await infoService.DeleteHomesubscriptionRequestAsync(guidRequestId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }
    }
}
