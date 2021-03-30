
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Services;

namespace SelfServiceLibrary.BL.Validation
{
    public class BookAddDTOValidator : AbstractValidator<BookAddDTO>
    {
        public BookAddDTOValidator(BookService service)
        {
            RuleFor(x => x.DepartmentNumber).NotEmpty().Length(1, 100);
            RuleFor(x => x.DepartmentNumber)
                .MustAsync(async (departmentNumber, _) => !await service.Exists(departmentNumber ?? string.Empty))
                .WithMessage("Department Number must be unique.");
            RuleFor(x => x.PublicationType).NotEmpty().Length(1, 100);
            RuleFor(x => x.Entered).NotEmpty();
        }
    }
}
