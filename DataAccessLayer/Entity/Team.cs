using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entity
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }


        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}
