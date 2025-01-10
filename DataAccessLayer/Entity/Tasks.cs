using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entity
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }


        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int EstimatedLoad { get; set; } 
        public int Difficulty { get; set; } 
    }

}
