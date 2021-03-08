
using AutoMapper;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Service.DTO.BookStatus;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class BookStatusProfile : Profile
    {
        public BookStatusProfile()
        {
            CreateMap<BookStatus, BookStatusListDTO>();
            CreateMap<BookStatusCreateDTO, BookStatus>(MemberList.Source);
            CreateMap<BookStatusUpdateDTO, BookStatus>(MemberList.Source);
        }
    }
}
