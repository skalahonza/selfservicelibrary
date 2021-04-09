
using Microsoft.Extensions.Options;

using OtpNet;

using SelfServiceLibrary.Web.Interfaces;
using SelfServiceLibrary.Web.Options;

namespace SelfServiceLibrary.Web.Services
{
    public class TimeBasedOneTimePasswordService : IOneTimePasswordService
    {
        private readonly Totp _totp;

        public TimeBasedOneTimePasswordService(IOptions<TotpOptions> options) => 
            _totp = new Totp(Base32Encoding.ToBytes(options.Value.SecretKey), totpSize: 10, step: 15);

        public string Generate() =>
            _totp.ComputeTotp();

        public bool Verify(string? otp)
        {
            var window = new VerificationWindow(previous: 1, future: 0);
            return _totp.VerifyTotp(otp, out var timeStepMatched, window);
        }
    }
}
