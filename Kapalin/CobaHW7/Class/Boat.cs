<<<<<<< HEAD
﻿using Postgrest.Attributes;
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
=======
﻿using System;
using System.Collections.Generic;

namespace CobaHW7.Class
{
    public class Boat
    {
        // ====== Properti lama (tetap dipertahankan untuk kompatibilitas) ======
        public int ID { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }         // harga/hari (legacy)
        public int Year { get; set; }
        public string Image { get; set; }          // path gambar (legacy)
        public bool Available { get; set; }        // flag ketersediaan umum
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // ====== Properti tambahan agar cocok dengan UI MVVM ======
        public string Name { get; set; }           // display name kapal
        public string Location { get; set; }       // lokasi/dermaga
        public int Capacity { get; set; }          // kursi/kapasitas
        public double Rating { get; set; }         // 0..5

        // Alias agar XAML kamu bisa binding properti baru tanpa mematikan yang lama
        public decimal PricePerDay                // dipakai UI
        {
            get => Price;
            set => Price = value;
        }
        public string ThumbnailPath               // dipakai UI
        {
            get => Image;
            set => Image = value;
        }

        public List<string> Facilities { get; set; } = new();
        public List<DateRange> Booked { get; set; } = new(); // jadwal penuh

        // ====== Konstruktor ======
        public Boat() { } // penting untuk data binding WPF

        public Boat(int id, string model, decimal price, int year, string image, bool available)
        {
            ID = id;
            Model = model;
            Price = price;
            Year = year;
            Image = image;
            Available = available;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

            // default yang masuk akal
            Name = model;
            Location = "";
            Capacity = 0;
            Rating = 0.0;
        }

        // Factory sederhana (opsional legacy)
        public static Boat Create(Dictionary<string, object> data)
        {
            return new Boat(
                (int)data["ID"],
                data["Model"]?.ToString(),
                Convert.ToDecimal(data["Price"]),
                (int)data["Year"],
                data["Image"]?.ToString(),
                Convert.ToBoolean(data["Available"])
            );
        }

        // ====== Helper & kompatibilitas ======
        public void SetAvailability(bool available)
        {
            Available = available;
            UpdatedAt = DateTime.Now;
        }

        // Overload: cek ketersediaan pakai rentang tanggal (dipakai filter UI)
        public bool IsAvailable(DateTime? start, DateTime? end)
        {
            if (!Available) return false;                 // kalau kapal di-nonaktifkan
            if (start == null || end == null) return true;
            if (start >= end) return false;

            foreach (var r in Booked)
                if (r.Overlaps(start.Value, end.Value)) return false;

            return true;
        }

        // Versi lama (retur flag global) tetap dipertahankan:
        public bool IsAvailable(DateTime startDate, DateTime endDate) => IsAvailable((DateTime?)startDate, (DateTime?)endDate);

        public void AddBooking(DateTime start, DateTime end)
        {
            if (start >= end) throw new ArgumentException("End harus > Start");
            Booked.Add(new DateRange(start, end));
            UpdatedAt = DateTime.Now;
        }

        public decimal CalculatePrice(DateTime startDate, DateTime endDate, decimal? discount = null)
        {
            int days = Math.Max(1, (endDate.Date - startDate.Date).Days);
            decimal total = PricePerDay * days;

            if (discount.HasValue) total -= discount.Value;
            return total;
        }

        public override string ToString()
            => $"ID:{ID}, Name:{Name ?? Model}, Price/Day:{PricePerDay}, Year:{Year}, Available:{Available}";
    }
}
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
