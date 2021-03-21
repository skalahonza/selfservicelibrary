using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.Web.Options
{
    public class AdminOptions
    {
        [Required]
        [MinLength(1)]
        public HashSet<string> Admins { get; set; }
    }
}
