using System.Collections.Generic;

namespace SelfServiceLibrary.Service.DTO.Book
{
    public class BookDetailDTO
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public List<string> CoAuthors { get; set; } = new List<string>();
        public string? PublicationType { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public string? DepartmentNumber { get; set; }
    }
}
