using SelfServiceLibrary.BL.Filters;

namespace SelfServiceLibrary.Web.Filters
{
    public class BooksFilter : IBooksFilter
    {
        public string? Departmentnumber { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Status { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
