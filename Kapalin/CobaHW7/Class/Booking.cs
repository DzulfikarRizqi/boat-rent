using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace CobaHW7.Class
{
    [Table("Bookings")]
    public partial class Booking : BaseModel
    {
        [PrimaryKey("BookingId", false)]
        [Column("BookingId")]
        public long BookingId { get; set; }

        [Column("BoatId")]
        public long BoatId { get; set; }

        [Column("UserID")]
        public string UserId { get; set; } = "";

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        [Column("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = "";

        [Column("Status")]
        public string Status { get; set; } = "";
    }
}
