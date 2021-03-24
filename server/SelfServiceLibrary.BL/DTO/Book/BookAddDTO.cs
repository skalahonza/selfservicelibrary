using System;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookAddDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? PublicationType { get; set; }
        public string? EnteredBy { get; set; }
        public DateTime? Entered { get; set; }
    }
}
