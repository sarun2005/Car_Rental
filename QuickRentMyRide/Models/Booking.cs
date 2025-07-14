namespace QuickRentMyRide.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        public int CustomerID { get; set; }

        public int CarID { get; set; }

        public string Return_Date { get; set; }

        public string Pickup_Date { get; set; }

        public string Total_Cost { get; set; }
    }
}
