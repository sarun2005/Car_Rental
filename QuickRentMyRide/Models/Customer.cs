using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace QuickRentMyRide.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        public string Full_Name { get; set; }

        public string Phone_Number { get; set; }

        public string Address { get; set; }

        public string DOB { get; set; }

        public string License_Photo { get; set; }

        public string IC_Number { get; set; }

        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }



        // Foreign key to User table
        public int? UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

    }
}
