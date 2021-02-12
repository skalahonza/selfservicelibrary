using AutoMapper;

using SelfServiceLibrary.Domain.Entities;
using SelfServiceLibrary.Service.DTO.Book;

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
