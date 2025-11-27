using System;

namespace CobaHW7.Class
{
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
            return !(end <= Start || start >= End);
        }
    }
}
