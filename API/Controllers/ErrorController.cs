using API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Returns an HTTP response with a specified status code and a corresponding API response object.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to include in the response.</param>
        /// <returns>An <see cref="IActionResult"/> containing a response object with the specified status code.</returns>
        [HttpGet]
        public IActionResult Error(int statusCode)
        {
            return new ObjectResult(new ResponseAPI(statusCode));
        }
    }
}
