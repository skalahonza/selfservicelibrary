using System.Collections.Generic;

using AspNetCore.Identity.Mongo.Model;

namespace SelfServiceLibrary.Card.Authentication.Model
{
    public class IdCard : MongoUser
    {
        public IdCard()
        {
            CvutUsername = string.Empty;
        }

        public IdCard(string cardNumber, string cvutUsername)
            : base(cardNumber)
        {
            CvutUsername = cvutUsername;
        }

        /// <summary>
        /// One time passwords.
        /// </summary>
        public HashSet<string> Otps { get; set; } = new HashSet<string>();

        public string CvutUsername { get; set; }
    }
}
