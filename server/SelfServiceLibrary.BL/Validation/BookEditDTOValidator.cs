
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Book;

namespace SelfServiceLibrary.BL.Validation
{
    public class BookEditDTOValidator : AbstractValidator<BookEditDTO>
    {
        public BookEditDTOValidator()
        {
            RuleFor(x => x.Name).Length(0, 100);
            RuleFor(x => x.PublicationType).NotEmpty().Length(1, 100);
            RuleFor(x => x.Publication).GreaterThan(0);
            RuleFor(x => x.Pages).GreaterThan(0);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
}
