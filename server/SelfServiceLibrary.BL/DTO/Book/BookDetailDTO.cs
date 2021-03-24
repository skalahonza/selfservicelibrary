using System;
using System.Collections.Generic;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookDetailDTO
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public List<string> CoAuthors { get; set; } = new List<string>();
        public string? PublicationType { get; set; }
        public string? FormType { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public string? DepartmentNumber { get; set; }
        public string? SystemNumber { get; set; }
        public string? FelNumber { get; set; }
        public string? NFCIdent { get; set; }
        public string? BarCode { get; set; }
        public bool IsAvailable { get; set; }
        public string? Depended { get; set; }
        public string? Conference { get; set; }
        public string? Note { get; set; }
        public string? CountryOfPublication { get; set; }
        public int? Publication { get; set; }
        public string? Publisher { get; set; }
        public int? YearOfPublication { get; set; }
        public int? Pages { get; set; }
        public decimal? Price { get; set; }
        public string? MagazineNumber { get; set; }
        public int? MagazineYear { get; set; }
        public string? ISBNorISSN { get; set; }
        public bool StsLocal { get; set; }
        public bool StsUK { get; set; }
        public DateTime? Entered { get; set; }
        public string? EnteredBy { get; set; }

        public string? CurrentIssueId { get; set; }
        public string? CurrentIssueIssuedTo { get; set; }
        public DateTime? CurrentIssueIssueDate { get; set; }
        public DateTime? CurrentIssueExpiryDate { get; set; }

        public string? StatusName { get; set; }
        public bool StatusIsVissible { get; set; }
        public bool StatusCanBeBorrowed { get; set; }
    }
}
