
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Validation
{
    public class BookAddDTOValidator : AbstractValidator<BookAddDTO>
    {
        public BookAddDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(0, 100);
            RuleFor(x => x.Author).NotEmpty().Length(0, 100);
            RuleFor(x => x.ISBN).NotEmpty().Matches(@"^(97(8|9))?\d{9}(\d|X)$").WithMessage("Invalid ISBN. Valid ISBNS formats are ISBN10 or ISBN13.");
        }
    }
}
