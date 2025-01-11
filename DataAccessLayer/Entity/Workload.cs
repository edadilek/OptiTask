using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class Workload
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public double workload {  get; set; }

        public User user { get; set; } = null!;
    }
}
