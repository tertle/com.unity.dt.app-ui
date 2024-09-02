using NUnit.Framework;
using Unity.AppUI.Core;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(DateRange))]
    class DateRangeTests
    {
        [Test]
        public void DateRange_Constructors()
        {
            var date1 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date2 = new DateRange(new System.DateTime(2020, 1, 1), new System.DateTime(2020, 1, 2));

            Assert.That(date1, Is.EqualTo(date2));
            Assert.That(date1.start.year, Is.EqualTo(2020));
            Assert.That(date1.start.month, Is.EqualTo(1));
            Assert.That(date1.start.day, Is.EqualTo(1));
            Assert.That(date1.end.year, Is.EqualTo(2020));
            Assert.That(date1.end.month, Is.EqualTo(1));
            Assert.That(date1.end.day, Is.EqualTo(2));
        }

        [Test]
        public void DateRange_Constructors_ArgumentOutOfRangeException_WhenStartIsGreaterThanEnd()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new DateRange(new Date(2020, 1, 2), new Date(2020, 1, 1)));
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new DateRange(new System.DateTime(2020, 1, 2), new System.DateTime(2020, 1, 1)));
        }

        [Test]
        public void DateRange_TryParse()
        {
            var date = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));

            Assert.That(DateRange.TryParse("2020-01-01,2020-01-02", out var result1), Is.True);
            Assert.That(result1, Is.EqualTo(date));

            Assert.That(DateRange.TryParse("2020-01-01/2020-01-02", out _), Is.False);
            Assert.That(DateRange.TryParse("2020-01-01,2020-01-02,2020-01-03", out _), Is.False);
            Assert.That(DateRange.TryParse("2020-01-01", out _), Is.False);

            Assert.That(DateRange.TryParse("2020-01-03,2020-01-02", out _), Is.False);
            Assert.That(DateRange.TryParse("2020-01-03,2020=01-02", out _), Is.False);
            Assert.That(DateRange.TryParse(null, out _), Is.False);
            Assert.That(DateRange.TryParse(string.Empty, out _), Is.False);
        }

        [Test]
        public void DateRange_ToString()
        {
            var date = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));

            Assert.That(date.ToString(), Is.EqualTo("2020-01-01,2020-01-02"));
        }

        [Test]
        public void DateRange_HashCode()
        {
            var date1 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date2 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date3 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 4));

            Assert.That(date1.GetHashCode(), Is.EqualTo(date2.GetHashCode()));
            Assert.That(date1.GetHashCode(), Is.Not.EqualTo(date3.GetHashCode()));
            Assert.That(date2.GetHashCode(), Is.Not.EqualTo(date3.GetHashCode()));
        }

        [Test]
        public void DateRange_Equals()
        {
            var date1 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date2 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date3 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 4));

            Assert.AreEqual(date1, date2);
            Assert.That(date1, Is.Not.EqualTo(date3));
        }

        [Test]
        public void DateRange_OperatorEquals()
        {
            var date1 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date2 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date3 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 4));

            Assert.That(date1 == date2, Is.True);
            Assert.That(date1 == date3, Is.False);
        }

        [Test]
        public void DateRange_OperatorNotEquals()
        {
            var date1 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date2 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));
            var date3 = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 4));

            Assert.That(date1 != date2, Is.False);
            Assert.That(date1 != date3, Is.True);
        }
    }
}
