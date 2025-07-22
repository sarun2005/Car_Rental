using System.ComponentModel.DataAnnotations.Schema;

namespace QuickRentMyRide.Models
{
    public class Car
    {
        public int CarID { get; set; }

        public string Rentperday { get; set; }

        public string Car_Model { get; set; }

        public string Car_Brand { get; set; }

        public string IsAvailable { get; set; }

        public string Numberplate { get; set; }

        public string Car_Image { get; set; }




        // Foreign Key to Admin table
        public int AdminID { get; set; }
        [ForeignKey("AdminID")]
        public virtual Admin Admin { get; set; }
    }
}
