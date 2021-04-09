namespace SelfServiceLibrary.Web.Interfaces
{
    public interface IOneTimePasswordService
    {
        string Generate();
        bool Verify(string otp);
    }
}
