
using FuncSharp;

namespace SelfServiceLibrary.BL.Responses
{
    public class BookReturned { }
    public class IssueNotFound { }
    public class BookAlreadyReturned { }

    public class ReturnResponse : Coproduct3<BookReturned, IssueNotFound, BookAlreadyReturned>
    {
        public ReturnResponse(BookReturned firstValue) : base(firstValue)
        {
        }

        public ReturnResponse(IssueNotFound secondValue) : base(secondValue)
        {
        }

        public ReturnResponse(BookAlreadyReturned thirdValue) : base(thirdValue)
        {
        }

        public ReturnResponse(ICoproduct3<BookReturned, IssueNotFound, BookAlreadyReturned> source) : base(source)
        {
        }

        protected ReturnResponse(int discriminator, object value) : base(discriminator, value)
        {
        }
    }
}
