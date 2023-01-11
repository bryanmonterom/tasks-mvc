namespace TasksMVC.Entities
{
    public class SubTask
    {
        public Guid Id { get; set; }
        public int  TaskId { get; set; }
        public Task Task { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int Position { get; set; }

    }
}
