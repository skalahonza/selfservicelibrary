namespace SelfServiceLibrary.BL.Exceptions.Business
{
    public class BookIsBorrowedException : BusinessLayerException
    {
        public BookIsBorrowedException(string departmentNumber)
            : base($"Book with department number {departmentNumber} is currently borrowed.")
        {
        }
    }
}
