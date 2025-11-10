using System;

namespace CobaHW7.Class
{
    // Rentang tanggal (End eksklusif) untuk cek bentrok booking.
    public class DateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateRange() { }
        public DateRange(DateTime start, DateTime end)
        {
            Start = start; End = end;
        }

        public bool Overlaps(DateTime start, DateTime end)
        {
            // true bila [start,end) bertumpuk dengan [Start,End)
            return !(end <= Start || start >= End);
        }
    }
}
