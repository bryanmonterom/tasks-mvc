using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Entities = TasksMVC.Entities;

namespace TasksMVC
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Task>().Property(a => a.Title).HasMaxLength(250).IsRequired();
        }

        public DbSet<Entities.Task> Tasks { get; set; }
        public DbSet<Entities.SubTask> SubTasks { get; set; }
        public DbSet<Entities.AttachedFile> AttachedFiles { get; set; }




    }
}
