using System;

using Microsoft.AspNetCore.Identity;

namespace SelfServiceLibrary.Card.Authentication.Options
{
    public class CardLoginTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public const string NAME = "CardLoginTokenProvider";

        public CardLoginTokenProviderOptions()
        {
            Name = NAME;
            TokenLifespan = TimeSpan.FromSeconds(30);
        }
    }
}
