using AutoMapper;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookAddDTO, Book>(MemberList.Source);
            CreateMap<BookEditDTO, Book>(MemberList.Source);
            CreateMap<Book, BookListDTO>();
            CreateMap<Book, BookSearchDTO>();
            CreateMap<Book, BookDetailDTO>();
        }
    }
}
