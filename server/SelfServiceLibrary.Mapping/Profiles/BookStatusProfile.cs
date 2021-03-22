
using AutoMapper;

using SelfServiceLibrary.BL.DTO.BookStatus;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class BookStatusProfile : Profile
    {
        public BookStatusProfile()
        {
            CreateMap<BookStatus, BookStatusListDTO>();
            CreateMap<BookStatusListDTO, BookStatusCreateDTO>();
            CreateMap<BookStatusListDTO, BookStatusUpdateDTO>();
            CreateMap<BookStatusCreateDTO, BookStatus>(MemberList.Source);
            CreateMap<BookStatusUpdateDTO, BookStatus>(MemberList.Source);
        }
    }
}
