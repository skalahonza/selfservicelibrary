namespace SelfServiceLibrary.BL.Exceptions.Business
{
    public class BookCannotBeBorrowedException : BusinessLayerException
    {
        public BookCannotBeBorrowedException(string departmentNumber)
            : base($"Book with department number {departmentNumber} cannot be borrowed at the moment.")
        {
        }
    }
}
