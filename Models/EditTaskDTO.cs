using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace TasksMVC.Models
{
    public class EditTaskDTO
    {
        [Required]
        [StringLength(250)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }

    }
}
