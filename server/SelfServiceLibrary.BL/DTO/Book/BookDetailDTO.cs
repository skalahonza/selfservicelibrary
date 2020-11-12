using System;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookDetailDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ISBN { get; set; }
    }
}
