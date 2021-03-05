
using FluentValidation;

using SelfServiceLibrary.Service.DTO.Issue;

namespace SelfServiceLibrary.Service.Validation
{

    public class NfcIssueReturnDTOValidator : AbstractValidator<NfcIssueReturnDTO>
    {
        public NfcIssueReturnDTOValidator()
        {
            RuleFor(x => x.BarCode).NotEmpty();
        }
    }
}
