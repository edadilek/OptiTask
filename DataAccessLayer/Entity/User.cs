using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}



