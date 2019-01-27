# DateBuckets

A C# class library built on top of [Time Period Library for .NET](https://www.codeproject.com/Articles/168662/Time-Period-Library-for-NET)

Returns buckets of time periods based upon the following rules:

 * days if range < 1 month, 
 * weeks if range < 6 months, 
 * months if range < 24 months, 
 * quarters if range < 60 months
 * years if range is 60 months or more
