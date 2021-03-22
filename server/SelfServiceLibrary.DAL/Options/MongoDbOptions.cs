using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.DAL.Options
{
    public class MongoDbOptions
    {
        [Required]
        public string? ConnectionString { get; set; }
        [Required]
        public string? DatabaseName { get; set; }
    }
}
