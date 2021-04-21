namespace SelfServiceLibrary.BL.Exceptions
{
    public class BookAlreadyExistsException : BusinessLayerException
    {
        public BookAlreadyExistsException(string departmentNumber)
            : base($"Book with department number {departmentNumber} already exists.")
        {
        }
    }
}
