using AutoMapper;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.Service.DTO.User;

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
