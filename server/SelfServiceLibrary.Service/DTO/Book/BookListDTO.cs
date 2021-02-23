namespace SelfServiceLibrary.Service.DTO.Book
{
    public class BookListDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? Name { get; set; }
        public string? PublicationType { get; set; }
        public string? Author { get; set; }
        public bool IsAvailable { get; set; }
    }
}
