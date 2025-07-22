using QuickRentMyRide.Services.Interfaces;

namespace QuickRentMyRide.Services.Implementations
{
    public class OtpService : IOtpService
    {
        private Random _random = new Random();

        public string GenerateOtp(int length = 6)
        {
            var otp = "";
            for (int i = 0; i < length; i++)
            {
                otp += _random.Next(0, 10).ToString();
            }
            return otp;
        }

        public bool ValidateOtp(string inputOtp, string actualOtp)
        {
            return inputOtp == actualOtp;
        }
    }
}
