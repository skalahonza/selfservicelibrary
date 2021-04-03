
using FluentValidation;

using SelfServiceLibrary.BL.DTO.Guest;

namespace SelfServiceLibrary.BL.Validation
{
    public class GuestDTOValidator : AbstractValidator<GuestDTO>
    {
        public GuestDTOValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();            
        }
    }
}
