namespace SelfServiceLibrary.Service.DTO.BookStatus
{
    public class BookStatusUpdateDTO
    {
        public string? Name { get; set; }

        public bool? IsVissible { get; set; }

        public bool? CanBeBorrowed { get; set; }
    }
}
