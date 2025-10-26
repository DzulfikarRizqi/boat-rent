using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Class
{
    internal class Bookings
    {
        private int _bookingId;
        private int _boatId;
        private int _userId;
        private DateTime _startDate;
        private DateTime _endDate;
        private decimal _totalAmount;
        private string _paymentMethod = string.Empty;
        private string _status;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public Bookings(int bookingId, int boatId, int userId, DateTime startDate, DateTime endDate)
        {
            _bookingId = bookingId;
            _boatId = boatId;
            _userId = userId;
            _startDate = startDate;
            _endDate = endDate;

            _status = "Pending";
            _createdAt = DateTime.Now;
            _updatedAt = DateTime.Now;
        }

        public int BookingId
        {
            get { return _bookingId; }
        }

        public int BoatId
        {
            get { return _boatId; }
        }

        public int UserId
        {
            get { return _userId; }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; _updatedAt = DateTime.Now; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; _updatedAt = DateTime.Now; }
        }

        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; }
        }

        public string Status
        {
            get { return _status; }
        }
        public void CancelBooking()
        {
            this._status = "Cancelled";
            this._updatedAt = DateTime.Now;
            Console.WriteLine("Booking ID: " + _bookingId + " telah dibatalkan.");
        }

        public void ConfirmPayment(string paymentMethod, decimal amount)
        {
            this._paymentMethod = paymentMethod;
            this._totalAmount = amount;
            this._status = "Confirmed";
            this._updatedAt = DateTime.Now;
            Console.WriteLine("Pembayaran untuk Booking ID: " + _bookingId + " telah dikonfirmasi.");
        }

        public int CalculateDurationInDays()
        {
            TimeSpan duration = _endDate - _startDate;
            return duration.Days;
        }
    }
}
