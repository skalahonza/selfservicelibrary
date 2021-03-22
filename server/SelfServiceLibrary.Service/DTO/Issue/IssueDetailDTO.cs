using System;

namespace SelfServiceLibrary.BL.DTO.Issue
{
    public class IssueDetailDTO
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Id { get; set; }
        public string DepartmentNumber { get; set; }
        public string BookName { get; set; }
        public string IssuedTo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }
}
