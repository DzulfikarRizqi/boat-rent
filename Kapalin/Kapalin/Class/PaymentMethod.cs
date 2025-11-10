namespace CobaHW7.Class
{
    public abstract class PaymentMethod
    {
        public string? Description { get; protected set; }
        public abstract void Pay(Booking booking);
    }

    public class CashPayment : PaymentMethod
    {
        public CashPayment() { Description = "Cash"; }

        public override void Pay(Booking booking)
        {
            booking.ConfirmPayment(PaymentKind.Cash, booking.TotalAmount);
        }
    }

    public class QrisPayment : PaymentMethod
    {
        public QrisPayment() { Description = "QRIS"; }

        public override void Pay(Booking booking)
        {
            booking.ConfirmPayment(PaymentKind.QRIS, booking.TotalAmount);
        }
    }
}
