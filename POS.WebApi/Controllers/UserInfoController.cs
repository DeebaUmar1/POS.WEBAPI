using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace POS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
  // This controller will work in case of Basic authentication
    public class UserInfoController : ControllerBase
    {
        // Function to get the current user's information
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }

            // Get the user claims
            var userClaims = User.Claims;

            // Extract user information from claims
            var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var userRole = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Return user information as a JSON object
            var userInfo = new
            {
                UserId = userId,
                UserName = userName,
                UserRole = userRole
            };

            return Ok(userInfo);
        }
    }

}
