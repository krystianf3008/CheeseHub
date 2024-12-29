using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role;
using CheeseHub.Models.User;
using CheeseHub.Models.User.DTOs;
using CheeseHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheeseHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IWebHostEnvironment environment, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            User user = await _userService.RegisterUser(model);
            return CreatedAtRoute(
            "GetUser",
                     new { Id = user.Id },
                     user);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto model)
        {

            User? user = await _userService.GetByEmail(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid login credentials");
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Password,user.PasswordHash) )
            {
                return BadRequest("Invalid login credentials");
            }
            
            (string jwt, string refresh) tokens = await _jwtService.GenerateTokens(user.Id);

            Response.Cookies.Append("JWT", tokens.jwt, new CookieOptions
            {
                HttpOnly = true
            });
            return Ok(new { token = tokens.jwt, refreshToken = tokens.refresh });

        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string userId, string token)
        {
            var valid = await _jwtService.ValidateRefreshToken(Guid.Parse(userId), token);
            if (!valid)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            var tokens = await _jwtService.GenerateTokens(Guid.Parse(userId));
            return Ok(new { token = tokens.JwtToken, refreshToken = tokens.RefreshToken });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            await _jwtService.RevokeRefreshToken(refreshToken);
            return Ok("Logged out successfully");
        }


        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> Get([FromBody] Guid id)
        {
            User user = await _userService.GetById(id);
            GetUserDTO model = new GetUserDTO
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                FirstName = user.FirstName,
                Role = user.Role.Name
            };
            return Ok(model);
        }
        [Authorize]
        [HttpGet("", Name = "GetMyData")]
        public async Task<IActionResult> GetMyData()
        {
            User user = await _userService.GetWithRole(id: Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value) );
            GetUserDTO model = new GetUserDTO
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                FirstName = user.FirstName,
                Role = user.Role.Name
            };
            return Ok(model);
        }
        [HttpGet("IsUserExistsByName/{name}", Name = "IsUserExistsByName")]
        public async Task<IActionResult> IsUserExistsByName([FromBody] string name)
        {
            
            return Ok(new {exists = await _userService.IsNameUnique(name) });
        }
        [HttpGet("IsTokenValid/{id}", Name = "IsTokenValid")]
        public async Task<IActionResult> IsTokenValid([FromRoute] string id)
        {
            if(await _userService.IsTokenValid(Guid.Parse(id)) == true  )
            {
                return Ok(new { success = true});
            }
            return Unauthorized();
        }
        [HttpGet("IsUserExistsByEmail/{name}", Name = "IsUserExistsByEmail")]
        public async Task<IActionResult> IsUserExistsByEmail([FromBody] string email)
        {

            return Ok(new { exists = await _userService.IsUserExistsByEmail(email) });
        }
    }

}
