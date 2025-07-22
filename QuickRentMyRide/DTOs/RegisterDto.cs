using System.ComponentModel.DataAnnotations;

namespace QuickRentMyRide.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Full_Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string Conform_Password { get; set; }
    }
}
 