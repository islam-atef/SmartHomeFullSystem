using API.ApiDTOs.HomeMangementDTOs.RequestDTOs;
using Application.Entities.SqlEntities.RoomEntities;
using Application.Home_Management.DTOs;
using Application.Home_Management.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeManagementController(IHomeService homeService) : ControllerBase
    {
        [HttpPost("Create-NewHome")]
        public async Task<IActionResult> CreateHome([FromBody] CreateHomeApiDTO homeDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            var deviceIP = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var home = new CreateHomeDTO
            {
                OwnerId = userId,
                HomeIP = deviceIP,
                Name = homeDTO.Name,
                Latitude = homeDTO.Latitude,
                Longitude = homeDTO.Longitude,
            };

            var result = await homeService.CreateNewHomeAsync(home);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpPatch("Rename-Home")]
        public async Task<IActionResult> RenameHome([FromBody] RenameApiDTO homeDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(homeDTO.HomeId, out var homeId))
                return BadRequest("Invalid Home ID");

            var home = new RenameHomeDTO
            {
                OwnerId = userId,
                NewName = homeDTO.NewName,
                HomeId = homeId
            };

            var result = await homeService.UpdateHomeNameAsync(home);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpGet("Get-HomeData/{homeId}")]
        public async Task<IActionResult> GetHomeData(string homeId)
        {
            if (!Guid.TryParse(homeId, out var  guidHomeId))
                return BadRequest("Invalid Home ID");

            var result = await homeService.GetHomeDataAsync(guidHomeId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpGet("Get-Home-SubRequest/{homeId}")]
        public async Task<IActionResult> GetHomeSubscriptionRequest(string homeId) 
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(homeId, out var guidHomeId))
                return BadRequest("Invalid Home ID");

            var result = await homeService.GerHomeAllSubscriptionRequestAsync(guidHomeId, userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpGet("Get-Home-NewSubRequest/{homeId}")]
        public async Task<IActionResult> GetHomeNewSubscriptionRequest(string homeId) 
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(homeId, out var guidHomeId))
                return BadRequest("Invalid Home ID");

            var result = await homeService.GerHomeNewSubscriptionRequestAsync(guidHomeId, userId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpPost("Add-NewRoom")]
        public async Task<IActionResult> AddNewRoom([FromBody] AddRoomDTO roomDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(roomDTO.HomeId, out var guidHomeId))
                return BadRequest("Invalid Home ID");


            var room = new CreateHomeRoomDTO
            {
                OwnerId = userId,
                HomeId = guidHomeId,
                RoomName = roomDTO.RoomName,
            };

            var result = await homeService.AddRoomToHomeAsync(room);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpPost("Add-NewUser")]
        public async Task<IActionResult> AddNewUser([FromBody] UserDTO userDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(userDTO.HomeId, out var guidHomeId))
                return BadRequest("Invalid Home ID");

            if (!Guid.TryParse(userDTO.NewUserId, out var newUserId))
                return BadRequest("Invalid User ID");


            var user = new HomeUser
            {
                OwnerId = userId,
                HomeId= guidHomeId,
                NewUserId = newUserId
            };

            var result = await homeService.AddUserToHomeAsync(user);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }



        [HttpDelete("Delete-User")]
        public async Task<IActionResult> DeleteUser([FromQuery] UserDTO userDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(userDTO.HomeId, out var guidHomeId))
                return BadRequest("Invalid Home ID");

            if (!Guid.TryParse(userDTO.NewUserId, out var existUserId))
                return BadRequest("Invalid User ID");


            var user = new HomeUser
            {
                OwnerId = userId,
                HomeId = guidHomeId,
                NewUserId = existUserId
            };

            var result = await homeService.DeleteUserFromHomeAsync(user);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }

        [HttpDelete("Delete-Room")]
        public async Task<IActionResult> DeleteRoom([FromQuery] DeleteRoomDTO roomDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid UserId claim");

            if (!Guid.TryParse(roomDTO.HomeId, out var guidHomeId))
                return BadRequest("Invalid Home ID");

            if (!Guid.TryParse(roomDTO.RoomId, out var roomId))
                return BadRequest("Invalid Room ID");


            var room = new DeleteHomeRoomDTO
            {
                OwnerId = userId,
                HomeId = guidHomeId,
                RoomId = roomId
            };

            var result = await homeService.DeleteHomeRoomAsync(room);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest($"{result.ErrorType.ToString()} : {result.ErrorMessage}");
        }
    }
}
