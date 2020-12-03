namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookAddDTO
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public int Quantity { get; set; }
    }
}
