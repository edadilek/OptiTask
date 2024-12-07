namespace OptiTask.DTOs
{
    public class ProjectDTO
    {
        public int TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public int ProjectId { get; set; }
    }
}
