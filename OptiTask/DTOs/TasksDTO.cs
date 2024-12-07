namespace OptiTask.DTOs
{
    public class TasksDTO
    {
        public int ProjectId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int EstimatedLoad { get; set; }
        public int Difficulty { get; set; }
    }
}
