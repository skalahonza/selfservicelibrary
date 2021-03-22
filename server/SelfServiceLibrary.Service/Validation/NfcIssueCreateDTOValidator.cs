
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.BL.Validation
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
