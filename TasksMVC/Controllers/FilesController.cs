using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TasksMVC.Entities;
using TasksMVC.Migrations;
using TasksMVC.Models;
using TasksMVC.Services;

namespace TasksMVC.Controllers
{
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAttachedFiles files;
        private readonly IUsersService usersService;
        private readonly string container = "attachedfiles";

        public FilesController(ApplicationDbContext context,
            IAttachedFiles files, IUsersService usersService)
        {
            this.context = context;
            this.files = files;
            this.usersService = usersService;
        }

        [HttpPost("{taskId:int}")]
        public async Task<ActionResult<IEnumerable<AttachedFile>>> Post(int taskId,
            [FromForm] IEnumerable<IFormFile> filesfromForm)
        {
            var idUser = usersService.GetUserId();
            var existsFiles = await context.AttachedFiles.AnyAsync(a => a.TaskId == taskId);
            var maxPosition = 0;

            var task = await context.Tasks.FirstOrDefaultAsync(a => a.Id == taskId && a.UserId == idUser);

            if (task is null)
            {

                return NotFound();
            }
            if (existsFiles)
            {
                maxPosition = await context.AttachedFiles
                    .Where(a => a.TaskId == taskId)
                    .Select(a => a.Position)
                    .MaxAsync();
            }

            var results = await files.Save(path: container, filesfromForm);

            var attachedFiles = results.Select((result, index) => new AttachedFile()
            {
                TaskId = taskId,
                CreatedDate = DateTime.UtcNow,
                Url = result.URL,
                Title = result.Title,
                Position = maxPosition + index + 1,
                Published = ""

            }).ToList();

            context.AddRange(attachedFiles);
            await context.SaveChangesAsync();

            return Ok(attachedFiles);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] string title)
        {
            var idUser = usersService.GetUserId();
            var file = await context.AttachedFiles.Include(a => a.Task).Where(a => a.Id == id).FirstOrDefaultAsync();

            if (file is null)
            {
                return NotFound();
            }

            if (file.Task.UserId != idUser) {
                return Forbid();
            }

            file.Title = title;
            await context.SaveChangesAsync();
            return Ok(file);



        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) {

            var idUser = usersService.GetUserId();
            var file = await context.AttachedFiles.Include(a => a.Task).Where(a => a.Id == id).FirstOrDefaultAsync();

            if (file is null)
            {
                return NotFound();
            }

            if (file.Task.UserId != idUser)
            {
                return Forbid();
            }

            context.Remove(file);
            await context.SaveChangesAsync();
            await files.Delete(file.Url, container);
            return Ok();


        }

    }


}
