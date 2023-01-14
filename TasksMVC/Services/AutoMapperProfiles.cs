using AutoMapper;
using TasksMVC.Models;

namespace TasksMVC.Services
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {

            CreateMap<TasksMVC.Entities.Task, TaskDTO>();
        }
    }
}
