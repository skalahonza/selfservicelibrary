
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.BL.Validation
{
    public class IssueCreateDTOValidator : AbstractValidator<IssueCreateDTO>
    {
        public IssueCreateDTOValidator()
        {
            RuleFor(x => x.DepartmentNumber).NotEmpty();
            RuleFor(x => x.ExpiryDate).NotEmpty();
        }
    }
}
