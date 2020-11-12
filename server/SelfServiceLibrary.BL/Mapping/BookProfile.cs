using AutoMapper;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Entities;

namespace SelfServiceLibrary.BL.Mapping
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookAddDTO, Book>().ForMember(x => x.Issues, x => x.Ignore());
            CreateMap<BookEditDTO, Book>().ForMember(x => x.Issues, x => x.Ignore());
            CreateMap<Book, BookListDTO>();
            CreateMap<Book, BookDetailDTO>();
        }
    }
}
