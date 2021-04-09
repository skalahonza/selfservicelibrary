using AutoMapper;

using SelfServiceLibrary.BL.DTO.Card;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<IdCard, CardListDTO>();
            CreateMap<AddCardDTO, IdCard>(MemberList.Source)
                .ForMember(x => x.HasPin, o => o.MapFrom(x => !string.IsNullOrEmpty(x.Pin)))
                .ForSourceMember(x => x.Pin, x => x.DoNotValidate())
                .ForSourceMember(x => x.PinConfirmation, x => x.DoNotValidate());
        }
    }
}
