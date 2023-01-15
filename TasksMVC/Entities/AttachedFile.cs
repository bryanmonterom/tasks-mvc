using Microsoft.EntityFrameworkCore;

namespace TasksMVC.Entities
{
    public class AttachedFile
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        [Unicode]
        public string Url { get; set; }
        public string Published { get; set; }

        public int Position { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
    }
}
