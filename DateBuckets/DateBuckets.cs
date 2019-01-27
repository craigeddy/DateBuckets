using System;
using System.Globalization;
using Itenso.TimePeriod;

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

            if (GetRange(start, end) == DateRange.Weeks)
            {
                return new DateDiff(start, end).Weeks + 1;
            }

            if (GetRange(start, end) == DateRange.Months)
            {
                var startMonth = start.Month;
                var endMonth = end.Month;
                if (start.Year == end.Year)
                {
                    return endMonth - startMonth + 1;
                }

                return 13 - startMonth + (end.Year - start.Year - 1) * 12 + endMonth;
            }

            if( GetRange(start,end) == DateRange.Quarters)
            {
                return new DateDiff(start,end).Quarters + 1;
            }

            return 0;

        }
    }
}