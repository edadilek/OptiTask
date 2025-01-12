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
    //Auth. endpoint işlemleri (login signup)

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository userRepository, UserService userService, AuthService authService, ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO creds)
        {
            //email veya password boş olmamalı
            if (string.IsNullOrEmpty(creds.Email) || string.IsNullOrEmpty(creds.Password)) {
                return BadRequest("Provide an appropiate body!");
            }

            var principal = await _authService.AuthenticateUserAsync(creds.Email, creds.Password, HttpContext);

            if (principal == null)
            {
                return Forbid("Try again to login!");
            }

            return Ok(new
            {
                Message = "Logged In"
            });
        }

        [HttpPost("/signup")]
        public async Task<IActionResult> Signup([FromBody] UserDTO userDTO)
        {
            //herhangi bir kısım boş olmamalı
            if(string.IsNullOrEmpty(userDTO.Name) || string.IsNullOrEmpty(userDTO.Surname) || string.IsNullOrEmpty(userDTO.Mail) || string.IsNullOrEmpty(userDTO.Password))
            {
                return BadRequest("Provide an appropiate body!");
            }

            //maile göre kullanıcı var mı diye kontrol ediyoruz
            var isExists = await _userRepository.CheckIfUserExists(userDTO.Mail);
            if (isExists)
            {
                return BadRequest("User exists!");
            }

            //yeni user oluşturuyoruz
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
           
            var principal = await _authService.AuthenticateUserAsync(userDTO.Mail, userDTO.Password, HttpContext);

            if (principal == null)
            {
                return Forbid("Try to Login");
            }

            return Ok(new
            {
                User = result.UserId,
                Message = "Signing In Successful"
            });
        }
    }
}
