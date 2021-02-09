using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;

namespace SelfServiceLibrary.API.Options
{
    public class MongoDbOptions : IOptions<MongoDbOptions>
    {
        public MongoDbOptions Value => this;
        [Required]
        public string ConnectionString { get; set; }
        [Required]
        public string DatabaseName { get; set; }
    }
}
