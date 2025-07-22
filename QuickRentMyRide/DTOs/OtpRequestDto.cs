using System.ComponentModel.DataAnnotations;

namespace QuickRentMyRide.DTOs
{
    public class OtpRequestDto
    {
        public string Email { get; set; }
        [Required(ErrorMessage = "OTP is required")]
        public string Otp { get; set; }
    }
}
