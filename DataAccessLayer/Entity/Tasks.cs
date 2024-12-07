using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int EstimatedLoad { get; set; }
        public int Difficulty { get; set; } 
        public Project Project { get; set; } = null!;
    }

}
