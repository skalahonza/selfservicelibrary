using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.Web.Options
{
    public class AdminOptions
    {
        [Required]
        [MinLength(1)]
#pragma warning disable CS8618 // Enforced by annotations
        public HashSet<string> Admins { get; set; }
#pragma warning restore CS8618 //  Enforced by annotations
    }
}
