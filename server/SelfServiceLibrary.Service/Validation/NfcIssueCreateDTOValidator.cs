
using FluentValidation;

using SelfServiceLibrary.Service.DTO.Issue;

namespace SelfServiceLibrary.Service.Validation
{
    public class NfcIssueCreateDTOValidator : AbstractValidator<NfcIssueCreateDTO>
    {
        public NfcIssueCreateDTOValidator()
        {
            RuleFor(x => x.BarCode).NotEmpty();
            RuleFor(x => x.ExpiryDate).NotEmpty();
        }
    }
}
