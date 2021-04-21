namespace SelfServiceLibrary.BL.Exceptions
{
    public class BookIsBorrowedException : BusinessLayerException
    {
        public BookIsBorrowedException(string departmentNumber)
            : base($"Book with department number {departmentNumber} is currently borrowed.")
        {
        }
    }
}
