using FuncSharp;

using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.BL.Responses
{
    public class BookNotFound { }

    public class BookAlreadyBorrowed { }

    public class BorrowResponse : Coproduct3<IssueDetailDTO, BookNotFound, BookAlreadyBorrowed>
    {
        public BorrowResponse(IssueDetailDTO firstValue) : base(firstValue)
        {
        }

        public BorrowResponse(BookNotFound secondValue) : base(secondValue)
        {
        }

        public BorrowResponse(BookAlreadyBorrowed thirdValue) : base(thirdValue)
        {
        }

        public BorrowResponse(ICoproduct3<IssueDetailDTO, BookNotFound, BookAlreadyBorrowed> source) : base(source)
        {
        }

        protected BorrowResponse(int discriminator, object value) : base(discriminator, value)
        {
        }
    }
}
