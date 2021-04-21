namespace SelfServiceLibrary.BL.Exceptions
{
    public class StatusAlreadyExistsException : BusinessLayerException
    {
        public StatusAlreadyExistsException(string name)
            : base($"Book status with name {name} already exists.")
        {
        }
    }
}
