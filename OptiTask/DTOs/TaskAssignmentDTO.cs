namespace OptiTask.DTOs
{
    public class TaskAssignmentDTO
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
