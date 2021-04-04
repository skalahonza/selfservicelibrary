using System.ComponentModel.DataAnnotations;

namespace CVUT.Usermap
{
    public class UsermapClientOptions
    {
#pragma warning disable CS8618 // Enforced by validation
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
#pragma warning restore CS8618 // Enforced by validation
    }
}
