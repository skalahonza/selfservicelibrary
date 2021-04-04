using AutoMapper;

using SelfServiceLibrary.BL.DTO.Guest;
using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class GuestProfile : Profile
    {
        public GuestProfile()
        {
            CreateMap<Guest, GuestDTO>().ReverseMap();
            CreateMap<Guest, UserInfoDTO>()
                .ForMember(x => x.Username, x => x.Ignore())
                .ForMember(x => x.FullName, x => x.Ignore());
        }
    }
}
