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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, UserService userService, AuthService authService, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO creds)
        {
            if (string.IsNullOrEmpty(creds.Email) || string.IsNullOrEmpty(creds.Password)) {
                return BadRequest("Provide an appropiate body!");
            }

            var principal = await _authService.AuthenticateUserAsync(creds.Email, creds.Password, HttpContext);

            if (principal == null)
            {
                return Forbid("Try again to login!");
            }

            HttpContext.User = principal;

            return Ok(new
            {
                Message = "Logged In"
            });
        }

        [HttpPost("/signup")]
        public async Task<IActionResult> Signup([FromBody] UserDTO userDTO)
        {
            if(string.IsNullOrEmpty(userDTO.Name) || string.IsNullOrEmpty(userDTO.Surname) || string.IsNullOrEmpty(userDTO.Mail) || string.IsNullOrEmpty(userDTO.Password))
            {
                return BadRequest("Provide an appropiate body!");
            }

            var isExists = await _userRepository.CheckIfUserExists(userDTO.Mail);
            if (isExists)
            {
                return BadRequest("User exists!");
            }


            var newUser = new User()
            {
                Name = userDTO.Name,
                Mail = userDTO.Mail,
                Surname = userDTO.Surname,
                Role = userDTO.Role
            };

            _userService.SetUserPassword(newUser, userDTO.Password);



            var result = await _userRepository.Create(newUser);

            _logger.LogInformation("User Created");
           
            await _authService.AuthenticateUserAsync(userDTO.Mail, userDTO.Password, HttpContext);

            return Ok(new
            {
                User = result.UserId,
                Message = "Signing In Successful"
            });
        }
    }
}
