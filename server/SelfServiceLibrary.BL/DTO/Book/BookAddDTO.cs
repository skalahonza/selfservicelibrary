using System;

using SelfServiceLibrary.DAL.Entities;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookAddDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? PublicationType { get; set; }
        public UserInfo? EnteredBy { get; set; }
        public DateTime? Entered { get; set; }
    }
}
