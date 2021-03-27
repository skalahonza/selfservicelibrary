namespace SelfServiceLibrary.BL.DTO.BookStatus
{
    public class BookStatusListDTO
    {
        public string? Name { get; set; }

        public bool IsVisible { get; set; }

        public bool CanBeBorrowed { get; set; }
    }
}
