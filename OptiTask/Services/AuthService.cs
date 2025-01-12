using DataAccessLayer.Interface;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace OptiTask.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;

        public AuthService(IUserRepository userRepository, UserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        //Token oluşturuyoruz
        public string GenerateToken (ClaimsIdentity claimIdentity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes("SuperSecretKeyAmAboutToGoCr@zySickOfThisRules");

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
                
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);
            if (string.IsNullOrEmpty(tokenString))
            {
                Console.WriteLine("WriteToken fonksiyonunda sorun var!!");
                return null;
            }
            Console.WriteLine("Token (GenerateToken): ", tokenString);
            return tokenString;
        }

        //Token doğrulaması
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes("SuperSecretKeyAmAboutToGoCr@zySickOfThisRules");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
            }, out SecurityToken validatedToken);

            if (validatedToken == null)
            {
                return false;
            }

            return true;
        }


        public async Task<ClaimsPrincipal> AuthenticateUserAsync(string username, string password, HttpContext httpContext)
        {
            var existedUser = await _userRepository.GetByEmail(username);

            if (existedUser == null)
            {
                return null;
            }

            //şifre kontrolü
            bool verifyPassword = _userService.VerifyUserPassword(existedUser, password);

            if (!verifyPassword)
            {
                return null;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, existedUser.UserId.ToString()),
                new Claim(ClaimTypes.Email, existedUser.Mail),
                new Claim(ClaimTypes.Role, existedUser.Role)
            };

            var identity = new ClaimsIdentity(claims);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            var token = GenerateToken(identity);

            httpContext.Response.Cookies.Append("jwt", token, new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddMinutes(5),
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true
            });

            return claimsPrincipal;
        }
    }
}
