using System;

namespace SelfServiceLibrary.Domain.DTO.Issue
{
    public class IssueCreateDTO
    {
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
