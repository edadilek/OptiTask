using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class TaskAssignment
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }

        public DateTime AssignedAt { get; set; }
        public Tasks Task { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
