using System;
using DateBuckets;
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
        [TestCase("1/3/2017", "12/1/2019", ExpectedResult = DateBucket.DateRange.Quarters)]
        [TestCase("1/3/2017", "1/31/2017", ExpectedResult = DateBucket.DateRange.Days)]
        [TestCase("1/3/2017", "5/31/2017", ExpectedResult = DateBucket.DateRange.Weeks)]
        [TestCase("1/3/2017", "1/31/2018", ExpectedResult = DateBucket.DateRange.Months)]

        public DateBucket.DateRange DateBucket_GetRange_ReturnsCorrectEnum(DateTime start, DateTime end)
        {
            return DateBucket.GetRange(start, end);
        }
    }
}
