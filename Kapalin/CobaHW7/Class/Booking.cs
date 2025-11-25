using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace CobaHW7.Class
{
    // Nama tabel "Bookings" harus sama persis dengan yang Anda buat di Supabase
    [Table("Bookings")]
    public partial class Booking : BaseModel
    {
        [PrimaryKey("BookingId", false)]
        [Column("BookingId")]
        public long BookingId { get; set; } // int8 (bigint)

        [Column("BoatId")]
        public long BoatId { get; set; } // int8 (bigint) - Foreign Key ke Boats(id)

        [Column("UserId")]
        public string UserId { get; set; } = ""; // UUID dari users table

        [Column("StartDate")]
        public DateTime StartDate { get; set; } // 'date' atau 'timestamp' di Supabase

        [Column("EndDate")]
        public DateTime EndDate { get; set; } // 'date' atau 'timestamp' di Supabase

        [Column("TotalAmount")]
        public decimal TotalAmount { get; set; } // 'numeric'

        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = ""; // Inisialisasi default

        [Column("Status")]
        public string Status { get; set; } = ""; // Inisialisasi default
    }
}
