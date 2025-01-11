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

        public int userId { get; set; }
        public double workload {  get; set; }
    }
}
