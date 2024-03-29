﻿using System.Collections.Generic;

namespace SelfServiceLibrary.BL.DTO.Book
{
    public class BookSearchDTO
    {
        public string? DepartmentNumber { get; set; }
        public string? Name { get; set; }
        public string? PublicationType { get; set; }
        public string? Author { get; set; }
        public List<string> CoAuthors { get; set; } = new List<string>();
        public List<string> Keywords { get; set; } = new List<string>();
        public bool IsAvailable { get; set; }
    }
}
