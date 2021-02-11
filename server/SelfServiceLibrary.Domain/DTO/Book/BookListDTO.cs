using System;

namespace SelfServiceLibrary.Domain.DTO.Book
{
    public class BookListDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ISBN { get; set; }
        public int Quantity { get; set; }
        public int Issued { get; set; }
        public bool IsAvailable => Issued < Quantity;
    }
}
