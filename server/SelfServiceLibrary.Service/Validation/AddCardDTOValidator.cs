﻿
using FluentValidation;

using System.Linq;

using SelfServiceLibrary.Service.DTO.Card;

namespace SelfServiceLibrary.Service.Validation
{
    public class AddCardDTOValidator : AbstractValidator<AddCardDTO>
    {
        public AddCardDTOValidator()
        {
            RuleFor(x => x.Number).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            When(x => !string.IsNullOrEmpty(x.Pin), () =>
            {
                RuleFor(x => x.Pin).Length(5);
                RuleFor(x => x.PinConfirmation).Must(x => string.IsNullOrEmpty(x) || x.All(char.IsDigit)).WithMessage("Pin can be numbers only.");
                RuleFor(x => x.PinConfirmation).Equal(x => x.Pin).WithMessage("Pin and Pin confirmation must match.");
            });
        }
    }
}