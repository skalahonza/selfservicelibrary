namespace SelfServiceLibrary.BL.Exceptions.Business
{
    public class BookAlreadyExistsException : BusinessLayerException
    {
        public BookAlreadyExistsException(string departmentNumber)
            : base($"Book with department number {departmentNumber} already exists.")
        {
        }
    }
}
