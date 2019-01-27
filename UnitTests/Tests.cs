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

        public int DateBucket_GetBucketCount_ReturnsCorrect_Quantity(DateTime start, DateTime end)
        {
            return DateBucket.GetBucketCount(start, end);
        }
    }
}
