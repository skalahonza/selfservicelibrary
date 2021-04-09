using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.Web.Options
{
    public class TotpOptions
    {
        [Required]
#pragma warning disable CS8618 // Enforced by annotations
        public string SecretKey { get; set; }
#pragma warning restore CS8618 //  Enforced by annotations
    }
}
