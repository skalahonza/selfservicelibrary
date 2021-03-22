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
                .ForSourceMember(x => x.Pin, x => x.DoNotValidate())
                .ForSourceMember(x => x.PinConfirmation, x => x.DoNotValidate());
            CreateMap<EditCardDTO, IdCard>(MemberList.Source)
                .ForSourceMember(x => x.Pin, x => x.DoNotValidate())
                .ForSourceMember(x => x.PinConfirmation, x => x.DoNotValidate());
        }
    }
}
