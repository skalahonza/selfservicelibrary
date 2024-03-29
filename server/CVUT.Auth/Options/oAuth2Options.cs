﻿using System.ComponentModel.DataAnnotations;

namespace CVUT.Auth.Options
{
    public class oAuth2Options
    {
#pragma warning disable CS8618 // Enforced by validation
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string RedirectUri { get; set; }
#pragma warning restore CS8618 // Enforced by validation
    }
}
