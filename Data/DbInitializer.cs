using Microsoft.AspNetCore.Mvc;
using VLCTraders.Api.Models.Common;
using VLCTraders.Api.Models.DTOs;
using VLCTraders.Api.Services;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors =
                    [
                        new ApiError { Field = "username", Message = "Username is required" },
                        new ApiError { Field = "password", Message = "Password is required" }
                    ]
                });
            }

            if (request.Username != "admin" || request.Password != "Password@123")
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid authentication token"
                });
            }

            var token = _jwtTokenService.GenerateToken(request.Username, "Admin");

            var response = new LoginResponse
            {
                Token = token,
                RefreshToken = "refresh-token-demo",
                ExpiresIn = 3600
            };

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = response
            });
        }
    }
}
