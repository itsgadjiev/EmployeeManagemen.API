using API.Database.Models;
using AutoMapper;

namespace API.DTOs.Employee.MappingProfile
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<CreateEmployeeDto, Database.Models.Employee>();
            CreateMap<UpdateEmployeeDto, Database.Models.Employee>();
        }
    }
}
