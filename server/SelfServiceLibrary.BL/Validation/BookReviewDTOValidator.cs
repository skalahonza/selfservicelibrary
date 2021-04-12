
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Book;
using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.BL.Validation
{
    public class BookReviewDTOValidator : AbstractValidator<BookReviewDTO>
    {
        private readonly IIssueService _issueService;

        public BookReviewDTOValidator(IIssueService issueService)
        {
            _issueService = issueService;
            RuleFor(x => x.DepartmentNumber).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Username)
                .MustAsync((dto, username, ct) => _issueService.HasRead(dto.DepartmentNumber!, username!))
                .WithMessage("User must read the book first before reviewing.");
            RuleFor(x => x.Value).InclusiveBetween(1, 10);
        }
    }
}
