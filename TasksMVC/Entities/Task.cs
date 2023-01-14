using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TasksMVC.Entities
{
    public class Task
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime CreatedDate { get; set;}
        public IEnumerable<SubTask> SubTasks { get; set; }
        public IEnumerable<AttachedFile> AttachedFiles { get; set; }

        public string UserId  { get; set; }
        public IdentityUser User { get; set; }

    }
}
