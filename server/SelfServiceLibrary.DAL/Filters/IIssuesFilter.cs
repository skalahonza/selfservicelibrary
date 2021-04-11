namespace SelfServiceLibrary.DAL.Filters
{
    public interface IIssuesFilter
    {
        /// <summary>
        /// Book department number or name
        /// </summary>
        string? Book { get; }

        /// <summary>
        /// Username or guest id
        /// </summary>
        string? IssuedTo { get; }
        /// <summary>
        /// Username
        /// </summary>
        string? IssuedBy { get; }
        /// <summary>
        /// Username
        /// </summary>
        string? ReturnedBy { get; }
        bool? IsReturned { get; }
    }
}
