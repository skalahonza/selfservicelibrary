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
            CreateMap<UserInfo, UserInfoDTO>().ReverseMap();
            CreateMap<CVUT.Usermap.Model.User, UserInfoDTO>()
                .ForMember(x => x.Email, o => o.MapFrom(x => x.PreferredEmail))
                .ForMember(x => x.TitleBefore, o => o.Ignore())
                .ForMember(x => x.TitleAfter, o => o.Ignore())
                .ForMember(x => x.PhoneNumber, o => o.Ignore());
        }
    }
}
