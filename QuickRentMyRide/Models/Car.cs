namespace QuickRentMyRide.Models
{
    public class Car
    {
        public int CarID { get; set; }

        public int AdminID { get; set; }

        public string Rentperday { get; set; }

        public string Car_Model { get; set; }

        public string Car_Brand { get; set; }

        public string IsAvailable { get; set; }

        public string Numberplate { get; set; }

        public string Car_Image { get; set; }
    }
}
