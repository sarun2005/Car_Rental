using System.ComponentModel.DataAnnotations;

namespace QuickRentMyRide.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }


        public string Role { get; set; }
    }
}
