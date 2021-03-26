﻿using System.Collections.Generic;

using AspNetCore.Identity.Mongo.Model;

namespace SelfServiceLibrary.Card.Authentication.Model
{
    public class IdCard : MongoUser
    {
        public IdCard()
        {
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

#pragma warning disable CS8618 // Enforced by validation.
        public string CvutUsername { get; set; }
#pragma warning restore CS8618 // Enforced by validation.
    }
}
