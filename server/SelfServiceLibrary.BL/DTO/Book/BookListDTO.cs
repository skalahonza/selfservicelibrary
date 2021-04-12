using System.Collections.Generic;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookListDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? SystemNumber { get; set; }
        public string? FelNumber { get; set; }
        public string? ISBNorISSN { get; set; }
        public string? Name { get; set; }
        public string? PublicationType { get; set; }
        public string? Storage { get; set; }
        public string? Author { get; set; }
        public List<string> CoAuthors { get; set; } = new List<string>();
        public int? YearOfPublication { get; set; }
        public int? Publication { get; set; }
        public int? Pages { get; set; }
        public string? FormType { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public string? Note { get; set; }
        public string? StatusName { get; set; }
        public bool StsLocal { get; set; }
        public bool StsUK { get; set; }
        public bool IsAvailable { get; set; }

        public int ReviewsCount { get; set; }
        public double? ReviewsAvg { get; set; }
    }
}
