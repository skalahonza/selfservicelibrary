using AutoMapper;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Service.DTO.Card;
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
