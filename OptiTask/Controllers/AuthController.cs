using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OptiTask.DTOs;
using OptiTask.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OptiTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, UserService userService, AuthService authService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userService = userService;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO creds)
        {
            if (string.IsNullOrEmpty(creds.Email) || string.IsNullOrEmpty(creds.Password)) {
                return BadRequest("Provide an appropiate body!");
            }

            var existedUser = await _userRepository.GetByEmail(creds.Email);
            if (existedUser == null)
            {
                return BadRequest("Email Not Registered");
            }


            bool verifyPassword = _userService.VerifyUserPassword(existedUser, creds.Password);

            if (!verifyPassword)
            {
                return BadRequest("Invalid crendentials!");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, existedUser.Mail),
                new Claim(ClaimTypes.Role, existedUser.Role),
            };

            string jwtToken = _authService.GenerateToken(claims);

            HttpContext.Response.Cookies.Append("jwt", jwtToken, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(2)
            });

            return Ok(new
            {
                User = existedUser.UserId,
                Message = "Logged In"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] UserDTO userDTO)
        {
            if(string.IsNullOrEmpty(userDTO.Name) || string.IsNullOrEmpty(userDTO.Surname) || string.IsNullOrEmpty(userDTO.Mail) || string.IsNullOrEmpty(userDTO.Password))
            {
                return BadRequest("Provide an appropiate body!");
            }

            var existedUser = _userRepository.GetByEmail(userDTO.Mail);
            if (existedUser != null)
            {
                return BadRequest("User exists!");
            }


            var newUser = new User()
            {
                Name = userDTO.Name,
                Mail = userDTO.Mail,
                Surname = userDTO.Surname,
                Role = "User"
            };

            _userService.SetUserPassword(newUser, userDTO.Password);



            var result = await _userRepository.Create(newUser);

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, result.Mail),
                new Claim(ClaimTypes.Role, result.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            HttpContext.Response.Cookies.Append("jwt", tokenString, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(2)
            });

            return Ok(new
            {
                User = result.UserId,
                Message = "Signing In Successful"
            });
        }
    }
}
