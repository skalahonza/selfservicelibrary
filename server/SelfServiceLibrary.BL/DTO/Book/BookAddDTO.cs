using System;

using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookAddDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? PublicationType { get; set; }
        public UserInfoDTO? EnteredBy { get; set; }
        public DateTime? Entered { get; set; }
    }
}
