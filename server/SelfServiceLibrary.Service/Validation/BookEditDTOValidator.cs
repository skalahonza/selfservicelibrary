
using FluentValidation;

using SelfServiceLibrary.Service.DTO.Book;

namespace SelfServiceLibrary.Service.Validation
{
    public class BookEditDTOValidator : AbstractValidator<BookEditDTO>
    {
        public BookEditDTOValidator()
        {
            RuleFor(x => x.Name).Length(0, 100);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        }
    }
}
