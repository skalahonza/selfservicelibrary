using AutoMapper;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserListDTO>();
        }
    }
}
