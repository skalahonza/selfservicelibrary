using System;

namespace SelfServiceLibrary.Service.DTO.Issue
{
    public class IssueCreateDTO
    {
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
