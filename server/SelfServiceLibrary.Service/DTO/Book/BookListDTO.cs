using System;
using System.Collections.Generic;

namespace SelfServiceLibrary.Service.DTO.Book
{
    public class BookListDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? SystemNumber { get; set; }
        public string? FelNumber { get; set; }
        public string? ISBNorISSN { get; set; }
        public string? Name { get; set; }
        public string? PublicationType { get; set; }
        public string? Depended { get; set; }
        public string? Author { get; set; }
        public List<string> CoAuthors { get; set; } = new List<string>();
        public int? YearOfPublication { get; set; }
        public int? Publication { get; set; }
        public int? Pages { get; set; }
        public string? FormType { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public string? Note { get; set; }
        public string? StatusName { get; set; }
        public bool IsAvailable { get; set; }

        public string? CurrentIssueIssuedTo { get; set; }
        public DateTime? CurrentIssueIssueDate { get; set; }
        public DateTime? CurrentIssueExpiryDate { get; set; }
    }
}
