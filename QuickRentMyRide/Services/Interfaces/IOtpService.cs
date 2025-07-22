namespace QuickRentMyRide.Services.Interfaces
{
    public interface IOtpService
    {
        string GenerateOtp(int length = 6);
        bool ValidateOtp(string inputOtp, string actualOtp);
    }
}
