
using FluentValidation;

using SelfServiceLibrary.Service.DTO.Issue;

namespace SelfServiceLibrary.Service.Validation
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
