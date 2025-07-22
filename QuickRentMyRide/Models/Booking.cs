using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickRentMyRide.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public string Return_Date { get; set; }

        public string Pickup_Date { get; set; }

        public decimal Total_Cost { get; set; }





        // Foreign Key to Customer table
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        // Foreign Key to Car table
        public int CarID { get; set; }
        [ForeignKey("CarID")]
        public virtual Car Car { get; set; }
    }
}
