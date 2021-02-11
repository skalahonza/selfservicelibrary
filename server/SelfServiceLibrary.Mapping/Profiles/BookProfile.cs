using AutoMapper;

using SelfServiceLibrary.Domain.DTO.Book;
using SelfServiceLibrary.Domain.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookAddDTO, Book>();
            CreateMap<BookEditDTO, Book>();
            CreateMap<Book, BookListDTO>();
            CreateMap<Book, BookDetailDTO>();
        }
    }
}
