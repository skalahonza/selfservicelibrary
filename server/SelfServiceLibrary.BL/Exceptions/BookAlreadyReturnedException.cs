namespace SelfServiceLibrary.BL.Exceptions
{
    public class BookAlreadyReturnedException : BusinessLayerException
    {
        public BookAlreadyReturnedException(string departmentNumber)
            : base($"Book with department number {departmentNumber} has already been returned.")
        {
        }
    }
}
