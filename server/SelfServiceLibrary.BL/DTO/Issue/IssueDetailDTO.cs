using System;
using System.Collections.Generic;
using System.Text;

namespace SelfServiceLibrary.BL.DTO.Issue
{
    public class IssueDetailDTO
    {

    }

    public class IssueListlDTO
    {

    }

    public class IssueCreateDTO
    {
        public Guid BookId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
