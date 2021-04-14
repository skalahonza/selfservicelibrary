using SelfServiceLibrary.DAL.Filters;

namespace SelfServiceLibrary.BG.Filters
{
    public class NotReturnedIssues : IIssuesFilter
    {
        public string? Book => null;

        public string? IssuedTo => null;

        public string? IssuedBy => null;

        public string? ReturnedBy => null;

        public bool? IsReturned => false;
    }
}
