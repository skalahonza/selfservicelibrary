using System;

using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.DTO.Issue
{
    public class IssueListDTO
    {
#pragma warning disable CS8618 // Items are not null in Issue entity
        public string Id { get; set; }
        public string DepartmentNumber { get; set; }
        public string BookName { get; set; }
        public UserInfoDTO IssuedTo { get; set; }
        public UserInfoDTO IssuedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
#pragma warning restore CS8618 // Items are not null in Issue entity
        public UserInfoDTO? ReturnedBy { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }
}
