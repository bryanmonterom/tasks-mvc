using AutoMapper;
using TasksMVC.Models;

namespace TasksMVC.Services
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<TasksMVC.Entities.Task, TaskDTO>()
            .ForMember(dto => dto.StepsTotal, ent => ent.MapFrom(x => x.SubTasks.Count()))
            .ForMember(dto => dto.StepsCompleted, ent => ent.MapFrom(x => x.SubTasks.Where(a => a.IsCompleted).Count()));
        }
    }
}
