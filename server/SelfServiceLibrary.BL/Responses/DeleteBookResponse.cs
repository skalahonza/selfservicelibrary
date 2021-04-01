using FuncSharp;

namespace SelfServiceLibrary.BL.Responses
{
    public class BookDeleted { }

    public class DeleteBookResponse : Coproduct3<BookDeleted, BookNotFound, BookIsBorrowed> {
        public DeleteBookResponse(BookDeleted firstValue) : base(firstValue)
        {
        }

        public DeleteBookResponse(BookNotFound secondValue) : base(secondValue)
        {
        }

        public DeleteBookResponse(BookIsBorrowed thirdValue) : base(thirdValue)
        {
        }

        public DeleteBookResponse(ICoproduct3<BookDeleted, BookNotFound, BookIsBorrowed> source) : base(source)
        {
        }

        protected DeleteBookResponse(int discriminator, object value) : base(discriminator, value)
        {
        }
    }
}
