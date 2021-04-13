using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.Email.Options
{
    public class SmtpNotificationServiceOptions
    {
        [Required]
        public string RelayAddress { get; set; } = "relay.felk.cvut.cz";
        [Required]
        public string From { get; set; } = "library@cyber.felk.cvut.cz";
        [Required]
        public string FromName { get; set; } = "Self Service Library";
    }
}
