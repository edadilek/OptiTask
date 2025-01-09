using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class ProjectTask
    {
        public int projectId {  get; set; }
        public int taskId { get; set; }

        public Project project { get; set; } = null!;
        public Tasks task { get; set; } = null!;
    }
}
