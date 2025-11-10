using System;

namespace CobaHW7.Class   // atau WpfApp1.Class kalau projectmu pakai itu
{
    public partial class Booking
    {
        public string? PaidWith { get; private set; }

        public void MarkAsPaid(string method)
        {
            if (Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking sudah dibatalkan.");

            Status = BookingStatus.Confirmed;
            PaidWith = method;
        }
    }

    public class BookingProcessor
    {
        public void ProsesPembayaran(Booking booking, PaymentMethod payment)
        {
            if (booking == null) throw new ArgumentNullException(nameof(booking));
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            payment.Pay(booking);
            Console.WriteLine($"Booking {booking.BookingId} dibayar dengan {payment.Description}");
        }
    }
}
