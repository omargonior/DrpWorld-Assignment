using AutoMapper;
using dpWorld.Application.DTOs.AttendanceDTO;
using dpWorld.Application.DTOs.UserDTO;
using dpWorld.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, ApplicationUser>();
            CreateMap<UpdateUserDto, ApplicationUser>();

            CreateMap<Attendance, AttendanceDto>();
        }
    }
}
