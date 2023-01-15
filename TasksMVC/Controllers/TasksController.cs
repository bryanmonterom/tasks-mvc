using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksMVC.Entities;
using TasksMVC.Models;
using TasksMVC.Services;
using Task = TasksMVC.Entities.Task;

namespace TasksMVC.Controllers
{
    [Route("/api/tasks/")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public TasksController(ApplicationDbContext context,
            IUsersService usersService, IMapper mapper)
        {
            this.context = context;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDTO>>> Get()
        {
            var idUser = usersService.GetUserId();
            var tasks = await context.Tasks.Where(t => t.UserId == idUser)
                .OrderBy(a => a.Order)
                .ProjectTo<TaskDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
            return tasks;

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Task>> Get(int id)
        {

            var idUser = usersService.GetUserId();

            var task = await context.Tasks.Include(t=> t.SubTasks.OrderBy(a=> a.Position)).FirstOrDefaultAsync(t => t.UserId == idUser && t.Id == id);

            if (task is null)
            {
                return NotFound();
            }
            return task;

        }



        [HttpPost]
        public async Task<ActionResult<Task>> Post([FromBody] string title)
        {

            var idUser = usersService.GetUserId();
            var taskExists = await context.Tasks.AnyAsync(t => t.UserId == idUser);
            var position = 0;

            if (taskExists)
            {
                position = await context.Tasks
                    .Where(a => a.UserId == idUser)
                    .Select(a => a.Order)
                    .MaxAsync();
            }
            var task = new Task()
            {
                Title = title,
                UserId = idUser,
                CreatedDate = DateTime.UtcNow,
                Order = position + 1
            };

            context.Add(task);
            await context.SaveChangesAsync();
            return task;

        }

        [HttpPost("Sort")]
        public async Task<IActionResult> Sort([FromBody] int[] ids)
        {

            var idUser = usersService.GetUserId();

            var tasks = await context.Tasks.Where(a => a.UserId == idUser).ToListAsync();

            var tasksId = tasks.Select(a => a.Id);

            var tasksDoesNotBelongs = tasksId.Except(ids).ToList();

            if (tasksDoesNotBelongs.Any())
            {
                return Forbid();
            }

            var tasksDictionary = tasks.ToDictionary(a => a.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var task = tasksDictionary[id];
                task.Order = i + 1;

            }

            await context.SaveChangesAsync();

            return Ok();

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditTask(int id, [FromBody] EditTaskDTO editTaskDTO)
        {
            var idUser = usersService.GetUserId();

            var task = await context.Tasks.FirstOrDefaultAsync(a => a.Id == id && a.UserId == idUser);

            if (task is null) {
                return NotFound();
            }

            task.Title = editTaskDTO.Title;
            task.Description = editTaskDTO.Description;

            await context.SaveChangesAsync();


            return Ok();



        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id) { 
        
            var idUser = usersService.GetUserId(); 
            var task = await context.Tasks.FirstOrDefaultAsync(a => a.Id == id && a.UserId == idUser);
            if (task is null)
            {
                return NotFound();
            }

            context.Tasks.Remove(task);
            await  context.SaveChangesAsync();
            return Ok();

        }


    }



}
