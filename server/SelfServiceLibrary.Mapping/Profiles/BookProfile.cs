using System.Linq;

using AutoMapper;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.CSV;
using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookAddDTO, Book>(MemberList.Source);
            CreateMap<Book, BookListDTO>();
            CreateMap<Book, BookSearchDTO>();
            CreateMap<Book, BookDetailDTO>();
            CreateMap<BookDetailDTO, BookEditDTO>();

            // CSV
            CreateMap<Book, BookCsvDTO>()
                .ForMember(x => x.IntStatus, o => o.MapFrom(x => x.Status.Name));

            CreateMap<BookCSV, BookCsvDTO>()
                .ForMember(x => x.CoAuthors, o => o.MapFrom(x => x.CoAuthors.Split(",", default).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList()))
                .ForMember(x => x.Publication, o => o.MapFrom(x => TryParseInt(x.Publication.Split(".", default).FirstOrDefault())))
                .ForMember(x => x.Keywords, o => o.MapFrom(x => x.Keywords.Split(",", default).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList()))
                .ForMember(x => x.StsLocal, o => o.MapFrom(x => !string.IsNullOrEmpty(x.StsLocal)))
                .ForMember(x => x.StsUK, o => o.MapFrom(x => !string.IsNullOrEmpty(x.StsUK)))
                .ForMember(x => x.Price, o => o.MapFrom(x => TryParseDecimal(x.Price)))
                .ReverseMap()
                .ForMember(x => x.CoAuthors, o => o.MapFrom(x => string.Join(',', x.CoAuthors)))
                .ForMember(x => x.Keywords, o => o.MapFrom(x => string.Join(',', x.Keywords)));
        }

        private static int? TryParseInt(string value)
        {
            if (int.TryParse(value, out var number))
                return number;
            return null;
        }

        private static double? TryParseDouble(string value)
        {
            if (double.TryParse(value, out var number))
                return number;
            return null;
        }

        private static decimal? TryParseDecimal(string value)
        {
            if (decimal.TryParse(value, out var number))
                return number;
            return null;
        }
    }
}
