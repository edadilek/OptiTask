using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TeamProject> TeamProjects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Workload> Workloads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users").HasKey(u => u.UserId);
            modelBuilder.Entity<Tasks>().ToTable("Tasks").HasKey(t => t.TaskId);
            modelBuilder.Entity<Team>().ToTable("Teams").HasKey(t => t.TeamId);
            modelBuilder.Entity<TeamMember>().ToTable("TeamMembers").HasKey(tm => new { tm.TeamId, tm.UserId });
            modelBuilder.Entity<Project>().ToTable("Projects").HasKey(p => p.ProjectId);
            modelBuilder.Entity<TaskAssignment>().ToTable("TaskAssignments").HasKey(ta => new { ta.TaskId, ta.UserId });
            modelBuilder.Entity<TeamProject>().ToTable("TeamProjects").HasKey(tp => new { tp.projectId, tp.teamId });
            modelBuilder.Entity<ProjectTask>().ToTable("ProjectTasks").HasKey(pt => new { pt.projectId, pt.taskId });
            modelBuilder.Entity<Workload>().ToTable("Workloads").HasKey(w => new { w.Id, w.userId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
