<<<<<<< HEAD
﻿using Postgrest.Attributes;
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
        public long UserId { get; set; } // Asumsi ini juga int8 (bigint)

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
=======
﻿using System;
using System.ComponentModel;

namespace CobaHW7.Class
{
    public enum BookingStatus { Pending, Confirmed, Cancelled }
    public enum PaymentKind { None, QRIS, VirtualAccount, Card, Cash }
    public partial class Booking : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int BookingId { get; set; }
        public int BoatId { get; set; }
        public int UserId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalAmount { get; set; }
        public PaymentKind PaymentMethod { get; set; } = PaymentKind.None;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Booking() { }

        public Booking(int bookingId, int boatId, int userId, DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                endDate = startDate.AddDays(1);

            BookingId = bookingId;
            BoatId = boatId;
            UserId = userId;
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }

        public int DurationDays => Math.Max(1, (EndDate - StartDate).Days);

        public bool Overlaps(DateTime s, DateTime e)
            => !(e <= StartDate || s >= EndDate);

        public void Reschedule(DateTime start, DateTime end)
        {
            if (Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking sudah dibatalkan.");

            if (start >= end)
                throw new ArgumentException("End harus > Start");

            StartDate = start.Date;
            EndDate = end.Date;
            Touch();
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        public void ConfirmPayment(PaymentKind method, decimal amount)
        {
            PaymentMethod = method;
            TotalAmount = amount;
            Status = BookingStatus.Confirmed;
            Touch();
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(PaymentMethod));
            OnPropertyChanged(nameof(TotalAmount));
        }

        public void Cancel()
        {
            Status = BookingStatus.Cancelled;
            Touch();
            OnPropertyChanged(nameof(Status));
        }

        private void Touch() => UpdatedAt = DateTime.Now;
    }
}
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
