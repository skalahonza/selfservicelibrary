using AutoMapper;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Entities;

namespace SelfServiceLibrary.BL.Mapping
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
