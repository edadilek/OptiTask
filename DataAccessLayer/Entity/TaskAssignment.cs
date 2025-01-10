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
