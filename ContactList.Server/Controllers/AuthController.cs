using ContactList.Shared.Dto;
using ContactList.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace ContactList.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetAllContacts() { return Ok(); }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> AddContact(ContactDto dto) { return Ok(); }

        private readonly UserManager<User> _userManager ;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;


        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto dto)
        {
            Console.WriteLine("www");
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.Username
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            
            if(user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                Email = user.Email
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            return Ok("User info endpoint is not implemented yet.");
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId == null)
            //    return Unauthorized();
            //var user = await _userManager.FindByIdAsync(userId);
            //if (user == null)
            //    return NotFound();
            //var userInfo = new UserDto
            //{
            //    Username = user.FullName,
            //    Email = user.Email
            //};
            //return Ok(userInfo);
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                //read about claims
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
