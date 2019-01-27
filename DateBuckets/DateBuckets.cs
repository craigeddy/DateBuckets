using System;
using System.Globalization;

namespace DateBuckets
{
    public class DateBucket
    {
        public enum DateRange
        {
            Days,
            Weeks,
            Months,
            Quarters
        }

        /// <summary>
        /// * tics should be
        /// *
        /// * days if range &lt; 1 month, 
        /// * weeks if range &lt; 6 months, 
        /// * months if range &lt; 24 months, 
        /// * quarters if range &lt; 60 months
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static DateRange GetRange(DateTime start, DateTime end)
        {
            if (end < start.AddMonths(1)) return DateRange.Days;
            if (end < start.AddMonths(6)) return DateRange.Weeks;
            return end < start.AddMonths(24) ? DateRange.Months : DateRange.Quarters;
        }

        public static int GetBucketCount(DateTime start, DateTime end)
        {
            if (GetRange(start, end) == DateRange.Days)
            {
                return (end - start).Days + 1;
            }

            var cal = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            var startWeek = cal.GetWeekOfYear(start, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            var endWeek = cal.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            if (startWeek > endWeek) startWeek = 1;
            return (endWeek - startWeek) + 1;
        }
    }
}