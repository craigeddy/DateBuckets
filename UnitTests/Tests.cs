using System;
using System.Linq;
using DateBuckets;
using Itenso.TimePeriod;
using NUnit.Framework;

/*
 * tics should be
 *
 * days if range < 1 month, 
 * weeks if range < 6 months, 
 * months if range < 24 months, 
 * quarters if range < 60 months
*/

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("1/3/2017", "1/31/2017", ExpectedResult = DateBucket.DateRange.Days)]
        [TestCase("1/3/2017", "5/31/2017", ExpectedResult = DateBucket.DateRange.Weeks)]
        [TestCase("1/3/2017", "1/31/2018", ExpectedResult = DateBucket.DateRange.Months)]
        [TestCase("1/3/2017", "12/1/2019", ExpectedResult = DateBucket.DateRange.Quarters)]
        [TestCase("1/3/2010", "12/1/2019", ExpectedResult = DateBucket.DateRange.Years)]

        public DateBucket.DateRange DateBucket_GetRange_ReturnsCorrectEnum(DateTime start, DateTime end)
        {
            return DateBucket.GetRange(start, end);
        }

        [Test]
        [TestCase("1/3/2017", "1/12/2017", ExpectedResult = 10)]
        [TestCase("1/3/2017", "1/30/2017", ExpectedResult = 28)]
        [TestCase("1/3/2019", "2/12/2019", ExpectedResult = 7)]
        [TestCase("12/31/2018", "2/12/2019", ExpectedResult = 7)]
        [TestCase("1/3/2017", "1/31/2018", ExpectedResult = 13)]
        [TestCase("1/31/2017", "1/3/2018", ExpectedResult = 13)]
        [TestCase("1/31/2017", "1/3/2019", ExpectedResult = 25)]
        [TestCase("1/31/2017", "4/3/2019", ExpectedResult = 10)]
        [TestCase("1/31/2017", "3/3/2019", ExpectedResult = 9)]
        [TestCase("12/3/2016", "12/1/2019", ExpectedResult = 13)]
        [TestCase("1/3/2010", "12/1/2019", ExpectedResult = 10)]
        [TestCase("12/3/2010", "11/1/2019", ExpectedResult = 10)]
        public int DateBucket_GetBucketCount_ReturnsCorrect_Quantity(DateTime start, DateTime end)
        {
            return DateBucket.GetBucketCount(start, end);
        }

        [Test]
        [TestCase("1/3/2017", "1/31/2017")]
        public void DateBucket_GetBucketsForDays_ReturnsDays(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection, Is.TypeOf<Days>());
        }

        [Test]
        [TestCase("1/3/2017", "1/31/2017")]
        public void DateBucket_GetBucketsForDays_Includes_Start(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection is Days days && days.GetDays().Any(d => d.Start == start));

        }

        [Test]
        [TestCase("1/3/2017", "1/31/2017")]
        public void DateBucket_GetBucketsForDays_Includes_End(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection is Days days && days.GetDays().Any(d => d.Start == end));
        }

        [Test]
        [TestCase("1/3/2017", "5/31/2017" /*DateBucket.DateRange.Weeks */)]
        public void DateBucket_GetBuckets_ForWeeks_ReturnsWeeks(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection, Is.TypeOf<Weeks>());
        }

        [Test]
        [TestCase("3/3/2017", "5/31/2017" /*DateBucket.DateRange.Weeks */)]
        public void DateBucket_GetBuckets_ForWeeks_FirstWeekStartsCorrectly(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection is Weeks weeks && 
                        weeks.GetWeeks().First().Start.Date == new DateTime(2017,2,26).Date);
        }

        [Test]
        [TestCase("1/3/2017", "5/31/2017" /*DateBucket.DateRange.Weeks */)]
        public void DateBucket_GetBuckets_ForWeeks_LastWeekEndsCorrectly(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            var weeks = (collection as Weeks)?.GetWeeks();
            Assert.That((weeks ?? throw new InvalidOperationException()).Last().End.Date == 
                        new DateTime(2017, 6, 3).Date);
        }

        [Test]
        [TestCase("1/3/2017", "1/31/2018" /* DateBucket.DateRange.Months */)]
        public void DateBucket_GetBuckets_ForMonths_ReturnsMonths(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection, Is.TypeOf<Months>());
        }

        [Test]
        [TestCase("3/3/2017", "1/31/2018", "3/1/2017" /* DateBucket.DateRange.Months */)]
        [TestCase("8/30/2017", "6/30/2018", "8/1/2017" /* DateBucket.DateRange.Months */)]
        public void DateBucket_GetBuckets_ForMonths_Returns_CorrectFirstMonth(DateTime start, DateTime end, DateTime expects)
        {
            var collection = DateBucket.GetBuckets(start, end);
            var firstDate = (collection as Months)?.GetMonths().First().Start.Date;
            Assert.That(firstDate, Is.EqualTo(expects.Date));                        
        }

        [Test]
        [TestCase("3/3/2017", "1/31/2018" /* DateBucket.DateRange.Months */)]
        [TestCase("8/30/2017", "6/30/2018" /* DateBucket.DateRange.Months */)]
        public void DateBucket_GetBuckets_ForMonths_Returns_CorrectFirstMonth(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            var months = (collection as Months)?.GetMonths().Count;
            Assert.That(months, Is.EqualTo(DateBucket.GetBucketCount(start,end) ));
        }

        [Test]
        [TestCase("3/3/2017", "1/31/2018", "6/3/2017", ExpectedResult = "June")]
        public string DateBucket_Get_Buckets_ForMonths_JuneDate_IsInJune(DateTime start, DateTime end, DateTime test)
        {
            var collection = DateBucket.GetBuckets(start, end);

            var range = new CalendarTimeRange(test, test.AddDays(1));
            var month = (collection as Months)?.GetMonths().FirstOrDefault(d => d.IntersectsWith(range)) as Month;

            return month?.MonthName;
        }

        [Test]
        [TestCase("1/3/2017", "12/1/2019" /* DateBucket.DateRange.Quarters */)]
        public void DateBucket_GetBuckets_ForQuarters_ReturnsQuarters(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection, Is.TypeOf<Quarters>());
        }

        [Test]
        [TestCase("12/3/2016", "12/1/2019" /* DateBucket.DateRange.Quarters */)]
        public void DateBucket_GetBuckets_ForQuarters_Returns_CorrectFirstMonth(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            var quarters = (collection as Quarters)?.GetQuarters();

            Assert.That(quarters != null && quarters.First().Start.Date == new DateTime(2016, 10, 1).Date);
        }

        [Test]
        [TestCase("1/3/2010", "12/1/2019")]
        [TestCase("12/3/2010", "11/1/2019")]
        public void DateBucket_GetBucketsForYears_ReturnsYears(DateTime start, DateTime end)
        {
            var collection = DateBucket.GetBuckets(start, end);
            Assert.That(collection, Is.TypeOf<Years>());
        }
    }
}
