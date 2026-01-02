using API.ApiDTOs.HomeMangementDTOs.RequestDTOs;
using API.ApiDTOs.UserInfoControllerDTOs.RequestDTOs;
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
                return Ok(result.Value);
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
                return Ok(result.Value);
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
                return Ok(result.Value);
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
                return Ok(result.Value);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpPatch("Update-PhoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumeber([FromBody] UpdatePhoneNumDTO phoneNumberDto) //[Done]
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.ChangeUserPhoneNumberAsync(phoneNumberDto.PhoneNumber, userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpPatch("Update-UserName")]
        public async Task<IActionResult> UpdateUserName([FromBody] UpdateUserNameDTO userNameDto) //[Done]
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.ChangeUserNameAsync(userNameDto.UserName, userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpPatch("Update-DisplayName")]
        public async Task<IActionResult> UpdateDiplayName([FromBody] UpdateDisplayNameDTO diplayNameDto) //[Done]
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var result = await infoService.ChangeUserDisplayNameAsync(diplayNameDto.DisplayName, userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpPatch("Update-UserImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateImage([FromForm] UpdateImageDTO imageDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (imageDto.Image == null || imageDto.Image.Length == 0)
                return BadRequest("Image is required");

            var result = await infoService.ChangeUserImageAsync(imageDto.Image, userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpPost("Subscribe-ToHome")]
        public async Task<IActionResult> SubscripeToHome([FromBody] SubscribeToHomeDTO homeDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (string.IsNullOrEmpty(homeDto.homeId))
                return BadRequest($" No Id provided: {homeDto.homeId}");
            Guid.TryParse(homeDto.homeId, out var guidHomeId);

            var result = await infoService.SendHomesubscriptionRequestAsync(userId,guidHomeId);
            if (result.IsSuccess)
                return Ok(result.Value);
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
                return Ok(result.Value);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }
    }
}
