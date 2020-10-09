using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.API.Options
{
    public class oAuth2Options
    {
        [Required]
        public string? ClientId { get; set; }
        [Required]
        public string? ClientSecret { get; set; }
        [Required]
        public string? RedirectUri { get; set; }
    }
}
