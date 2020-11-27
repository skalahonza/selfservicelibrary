using System;

namespace SelfServiceLibrary.BL.DTO.Issue
{
    public class IssueListlDTO
    {
        public Guid Id { get; set; }
        public string? BookName { get; set; }
        public string? ISBN { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
