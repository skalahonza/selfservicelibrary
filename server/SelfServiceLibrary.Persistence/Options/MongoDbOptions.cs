using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.Persistence.Options
{
    public class MongoDbOptions
    {
        [Required]
        public string? ConnectionString { get; set; }
        [Required]
        public string? DatabaseName { get; set; }
    }
}
