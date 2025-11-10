using System;
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
