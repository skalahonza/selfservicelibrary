
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Validation
{
    public class BookAddDTOValidator : AbstractValidator<BookAddDTO>
    {
        public BookAddDTOValidator()
        {
            RuleFor(x => x.DepartmentNumber).NotEmpty().Length(1, 100);
            RuleFor(x => x.PublicationType).NotEmpty().Length(1, 100);
            RuleFor(x => x.EnteredBy).NotEmpty().Length(1, 100);
            RuleFor(x => x.Entered).NotEmpty();
        }
    }
}
