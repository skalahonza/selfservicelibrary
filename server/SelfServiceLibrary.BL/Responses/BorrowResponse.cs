using FuncSharp;

using SelfServiceLibrary.BL.DTO.Issue;

namespace SelfServiceLibrary.BL.Responses
{
    public class BorrowResponse : Coproduct3<IssueDetailDTO, BookNotFound, BookIsBorrowed>
    {
        public BorrowResponse(IssueDetailDTO firstValue) : base(firstValue)
        {
        }

        public BorrowResponse(BookNotFound secondValue) : base(secondValue)
        {
        }

        public BorrowResponse(BookIsBorrowed thirdValue) : base(thirdValue)
        {
        }

        public BorrowResponse(ICoproduct3<IssueDetailDTO, BookNotFound, BookIsBorrowed> source) : base(source)
        {
        }

        protected BorrowResponse(int discriminator, object value) : base(discriminator, value)
        {
        }
    }
}
