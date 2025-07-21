using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

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

        [NotMapped]
        public Claim SomeClaim { get; set; }

        
    }
}
