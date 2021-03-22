using AutoMapper;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.Service.DTO.Book;

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
