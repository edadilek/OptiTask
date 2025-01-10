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
