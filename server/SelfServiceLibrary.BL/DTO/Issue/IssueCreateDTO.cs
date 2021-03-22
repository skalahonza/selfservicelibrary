using System;

namespace SelfServiceLibrary.BL.DTO.Issue
{
    public class IssueCreateDTO
    {
#pragma warning disable CS8618 // Enforced by validation
        public string DepartmentNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
#pragma warning restore CS8618 // Enforced by validation
    }
}
