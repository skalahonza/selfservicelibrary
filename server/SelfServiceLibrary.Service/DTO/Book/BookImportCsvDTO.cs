using System.Collections.Generic;

namespace SelfServiceLibrary.Service.DTO.Book
{
    public class BookImportCsvDTO
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public List<string> CoAuthors { get; set; } = new List<string>();
        public string? PublicationType { get; set; }
        public string? Depended { get; set; }
        public string? SystemNumber { get; set; }
        public string? FelNumber { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string DepartmentNumber { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string? BarCode { get; set; }
        public int? Pages { get; set; }
        public int? Publication { get; set; }
        public int? YearOfPublication { get; set; }
        public string? Publisher { get; set; }
        public string? CountryOfPublication { get; set; }
        public string? ISBNorISSN { get; set; }
        public string? MagazineNumber { get; set; }
        public int? MagazineYear { get; set; }
        public string? Conference { get; set; }
        public double? Price { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public string? Note { get; set; }
        public bool StsLocal { get; set; }
        public bool StsUK { get; set; }
    }
}
