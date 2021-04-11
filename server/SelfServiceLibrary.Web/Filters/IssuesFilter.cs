using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.DAL.Filters;

namespace SelfServiceLibrary.Web.Filters
{
    public record IssuesFilter : IIssuesFilter
    {
        public string? Book { get; set; }

        public UserInfoDTO? IssuedToInfo { get; set; }

        public string? IssuedTo => IssuedToInfo switch
        {
            { GuestId: string guestId } => guestId,
            { Username: string username } => username,
            _ => null
        };

        public UserInfoDTO? IssuedByInfo { get; set; }

        public string? IssuedBy => IssuedByInfo switch
        {
            { Username: string username } => username,
            _ => null
        };

        public UserInfoDTO? ReturnedByInfo { get; set; }

        public string? ReturnedBy => ReturnedByInfo switch
        {
            {Username: string username } => username,
            _ => null
        };

        public bool? IsReturned { get; set; }
    }
}
