
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using SelfServiceLibrary.Card.Authentication.Model;

namespace SelfServiceLibrary.Card.Authentication.Providers
{
    public class CardLoginTokenProvider : TotpSecurityStampBasedTokenProvider<IdCard>
    {
        public const string NAME = "CardLoginTokenProvider";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdCard> manager, IdCard user) =>
            Task.FromResult(false);

        /// <summary>
        /// Returns a constant, provider and user unique modifier used for entropy in generated tokens from user information.
        /// </summary>
        /// <param name="purpose">The purpose the token will be generated for.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}" /> that can be used to retrieve user properties.</param>
        /// <param name="user">The user a token should be generated for.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing a constant modifier for the specified 
        /// <paramref name="user" /> and <paramref name="purpose" />.
        /// </returns>
        public override Task<string> GetUserModifierAsync(string purpose, UserManager<IdCard> manager, IdCard user) =>
            // enrich entropy with username of the card owner
            Task.FromResult("Totp:" + purpose + ":" + user.Id + user.CvutUsername);
    }
}
