using Postgrest.Attributes;
using Postgrest.Models; // Penting untuk BaseModel
using System;

namespace CobaHW7.Class
{
    // [Table("NamaTabelDiSupabase")]
    // Nama tabel "Boats" harus sama persis dengan yang Anda buat di Supabase
    [Table("Boats")]
    public class Boat : BaseModel
    {
        // [PrimaryKey("nama_kolom_pk", false)]
        // 'false' berarti database yang mengatur nilainya (seperti auto-increment)
        [PrimaryKey("id", false)]
        [Column("id")]
        public long ID { get; set; } // int8 (bigint) di Supabase adalah 'long' di C#

        [Column("Name")]
        public string Name { get; set; } = ""; // Inisialisasi default untuk atasi CS8618

        [Column("Model")]
        public string Model { get; set; } = ""; // Inisialisasi default

        [Column("Location")]
        public string Location { get; set; } = ""; // Inisialisasi default

        [Column("Capacity")]
        public int Capacity { get; set; } // int4 (integer)

        [Column("Year")]
        public int Year { get; set; } // int4 (integer)

        [Column("Rating")]
        public double Rating { get; set; } // float8 (double)

        [Column("PricePerDay")]
        public decimal PricePerDay { get; set; } // 'numeric' di Supabase adalah 'decimal' di C#

        [Column("Available")]
        public bool? Available { get; set; } // bool

        [Column("ThumbnailPath")]
        public string ThumbnailPath { get; set; } = ""; // Inisialisasi default
    }
}
