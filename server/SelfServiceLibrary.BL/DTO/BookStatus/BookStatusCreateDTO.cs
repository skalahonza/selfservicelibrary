namespace SelfServiceLibrary.BL.DTO.BookStatus
{
    public class BookStatusCreateDTO
    {
        public string? Name { get; set; }

        public bool? IsVisible { get; set; }

        public bool? CanBeBorrowed { get; set; }
    }
}
