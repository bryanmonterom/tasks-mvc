namespace TasksMVC.Models
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public int StepsCompleted { get; set; }
        public int StepsTotal { get; set; }

    }
}
