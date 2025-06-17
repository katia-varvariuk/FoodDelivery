using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FoodDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result.Succeeded)
            {
                SetRefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result.Succeeded)
            {
                SetRefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }

            return Unauthorized(result);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto request = null)
        {
            var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh токен є обов'язковим" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var result = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

            if (result.Succeeded)
            {
                SetRefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }

            return Unauthorized(result);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RevokeToken([FromBody] TokenRequestDto request = null)
        {
            var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh токен є обов'язковим" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var result = await _authService.RevokeTokenAsync(refreshToken, ipAddress);

            if (!result)
            {
                return NotFound(new { message = "Токен не знайдено" });
            }

            return Ok(new { message = "Токен успішно відкликано" });
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var updatedUser = await _authService.UpdateUserAsync(userId, userDto);

            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7), // Повинно відповідати терміну дії токена
                SameSite = SameSiteMode.Strict,
                Secure = true // В продакшені має бути true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}