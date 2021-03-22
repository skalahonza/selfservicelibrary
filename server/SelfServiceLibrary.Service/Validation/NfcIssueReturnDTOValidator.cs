
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.BL.Validation
{
    public class NfcIssueReturnDTOValidator : AbstractValidator<NfcIssueReturnDTO>
    {
        public NfcIssueReturnDTOValidator()
        {
            RuleFor(x => x.BarCode).NotEmpty();
        }
    }
}
