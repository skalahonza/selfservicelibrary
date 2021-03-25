using FuncSharp;

namespace SelfServiceLibrary.BL.Responses
{
    public class BookCreated { }
    public class BookAlreadyExists { }

    public class CreateBookResponse : Coproduct2<BookCreated, BookAlreadyExists>
    {
        public CreateBookResponse(BookCreated firstValue) : base(firstValue)
        {
        }

        public CreateBookResponse(BookAlreadyExists secondValue) : base(secondValue)
        {
        }

        public CreateBookResponse(ICoproduct2<BookCreated, BookAlreadyExists> source) : base(source)
        {
        }

        protected CreateBookResponse(int discriminator, object value) : base(discriminator, value)
        {
        }
    }
}
