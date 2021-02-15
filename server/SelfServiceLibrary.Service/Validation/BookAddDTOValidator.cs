
using FluentValidation;

using SelfServiceLibrary.Service.DTO.Book;

namespace SelfServiceLibrary.Service.Validation
{
    public class BookAddDTOValidator : AbstractValidator<BookAddDTO>
    {
        public BookAddDTOValidator()
        {
            RuleFor(x => x.DepartmentNumber).NotEmpty().Length(0, 100);
            RuleFor(x => x.Name).NotEmpty().Length(0, 100);
            RuleFor(x => x.Author).NotEmpty().Length(0, 100);
        }
    }
}
