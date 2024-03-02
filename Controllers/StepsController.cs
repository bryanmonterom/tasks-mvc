using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TasksMVC.Entities;
using TasksMVC.Models;
using TasksMVC.Services;

namespace TasksMVC.Controllers
{
    [Route("api/steps/")]
    public class StepsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IUsersService usersService;

        public StepsController(ApplicationDbContext context, IUsersService usersService)
        {
            this.context = context;
            this.usersService = usersService;
        }

        [HttpPost("{taskId:int}")]
        public async Task<ActionResult<SubTask>> Post(int taskId, [FromBody] StepsCreateDTO stepsCreateDTO)
        {

            var iduser = usersService.GetUserId();
            var task = await context.Tasks.FirstOrDefaultAsync(a => a.Id == taskId && a.UserId == iduser);

            if (task == null)
            {
                return NotFound();
            }

            var maxPosition = 0;
            var existSteps = await context.SubTasks.AnyAsync(a => a.TaskId == taskId);

            if (existSteps)
            {
                maxPosition = await context.SubTasks
                    .Where(a => a.TaskId == taskId).Select(a => a.Position).MaxAsync();
            }

            var step = new SubTask()
            {
                TaskId = taskId,
                Position = maxPosition+1,
                Description= stepsCreateDTO.Description,
                IsCompleted=stepsCreateDTO.IsCompleted,
                
            };

            context.SubTasks.Add(step);
            context.SaveChanges();

            return Ok(step);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] StepsCreateDTO stepsCreateDTO) {
            var iduser = usersService.GetUserId();

            var step = await context.SubTasks.Include(t => t.Task).FirstOrDefaultAsync(s => s.Id == id);
            if (step is null) {

                return NotFound();
            }
            if (step.Task.UserId != iduser) {

                return Forbid();
            }

            step.Description = stepsCreateDTO.Description;
            step.IsCompleted = stepsCreateDTO.IsCompleted;

            await context.SaveChangesAsync();   

            return Ok();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id) { 
        
            var iduser = usersService.GetUserId();
            var step = await context.SubTasks.Include(t => t.Task).FirstOrDefaultAsync(s => s.Id == id);
            if (step is null)
            {

                return NotFound();
            }
            if (step.Task.UserId != iduser)
            {

                return Forbid();
            }

            context.SubTasks.Remove(step);
            await context.SaveChangesAsync();
            return Ok();    

        }

        [HttpPost("Sort/{taskId:int}")]
        public async Task<IActionResult> Sort(int taskId, [FromBody] Guid[] ids ) {

            var iduser = usersService.GetUserId();
            var task = await context.Tasks.FirstOrDefaultAsync(a => a.Id == taskId && a.UserId == iduser);
            if (task is null)
            {
                return NotFound();
            }

            var steps = await context.SubTasks.Where(a => a.TaskId == taskId).ToListAsync();

            var stepsIds = steps.Select(a => a.Id);

            var stepsNotBelongsToTask = stepsIds.Except(ids);

            if (stepsNotBelongsToTask.Any()) {

                return BadRequest("Missing steps");
            }

            var stepsDictionary = steps.ToDictionary(a => a.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var step = stepsDictionary[id];
                step.Position= i+1;
            }

            await context.SaveChangesAsync();   
            return Ok();

        }



    }
}
