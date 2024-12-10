using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Identity;

namespace OptiTask.Services
{
    public class UserService
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(PasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public void SetUserPassword(User user, string password)
        {
            user.Password = _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyUserPassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
