using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class TeamProject
    {
        public int teamId { get; set; }
        public int projectId { get; set; }

        public Project project { get; set; } = null!;
        public Team team { get; set; } = null!;
    }
}
