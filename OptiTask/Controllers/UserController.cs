using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using OptiTask.DTOs;
using DataAccessLayer.Repository;
using DataAccessLayer.Interface;
using Microsoft.AspNetCore.Authorization;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UserController(IUserRepository context)
        {
            userRepository = context;
        }

        // Kullanıcıları listeleme
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userRepository.GetAll();
            return Ok(users);
        }

        // Yeni kullanıcı oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            var newuser = new User()
            {
                Name = user.Name,
                Mail = user.Mail,
                Password = user.Password,
                Role = user.Role,
                Surname = user.Surname,

            };

            var result = await userRepository.Create(newuser);
            return Ok(result);
        }

        // Kullanıcı güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await userRepository.GetById(id);
            if (user == null) return NotFound();

            user.Name = updatedUser.Name;
            user.Surname = updatedUser.Surname;
            user.Mail = updatedUser.Mail;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role;

            await userRepository.Update(user);
            return Ok(user);
        }

        // Kullanıcı silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await userRepository.GetById(id);
            if (user == null) return NotFound();

            await userRepository.Delete(user);

            return Ok();
        }
    }
}

