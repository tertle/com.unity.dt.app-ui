using NUnit.Framework;
using Unity.AppUI.Core;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(Date))]
    class DateTests
    {
        [Test]
        public void Date_Constructors()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(new System.DateTime(2020, 1, 1));

            Assert.That(date1, Is.EqualTo(date2));
            Assert.That(date1.year, Is.EqualTo(2020));
            Assert.That(date1.month, Is.EqualTo(1));
            Assert.That(date1.day, Is.EqualTo(1));
        }

        [Test]
        public void Date_Now()
        {
            var date = Date.now;
            var dateTime = System.DateTime.Now;

            Assert.That(date.year, Is.EqualTo(dateTime.Year));
            Assert.That(date.month, Is.EqualTo(dateTime.Month));
            Assert.That(date.day, Is.EqualTo(dateTime.Day));
        }

        [Test]
        public void Date_Constructors_ArgumentOutOfRangeException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 0, 1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 13, 1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 1, 0));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 1, 32));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 2, 30));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 4, 31));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 6, 31));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 9, 31));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2020, 11, 31));
            Assert.DoesNotThrow(() => new Date(2020, 2, 29));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Date(2021, 2, 29));
        }

        [Test]
        public void Date_CastToDateTime()
        {
            var date = new Date(2020, 1, 1);
            var dateTime = (System.DateTime)date;

            Assert.That(dateTime.Year, Is.EqualTo(2020));
            Assert.That(dateTime.Month, Is.EqualTo(1));
            Assert.That(dateTime.Day, Is.EqualTo(1));
        }

        [Test]
        public void Date_ToString()
        {
            var date = new Date(2020, 1, 1);
            var date2 = new Date(2020, 10, 1);

            Assert.That(date.ToString(), Is.EqualTo("2020-01-01"));
            Assert.That(date2.ToString(), Is.EqualTo("2020-10-01"));
        }

        [Test]
        public void Date_HashCode()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1.GetHashCode(), Is.EqualTo(date2.GetHashCode()));
            Assert.That(date1.GetHashCode(), Is.Not.EqualTo(date3.GetHashCode()));
        }

        [Test]
        public void Date_Equals()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1.Equals(date2), Is.True);
            Assert.That(date1.Equals((object)date3), Is.False);
        }

        [Test]
        public void Date_OperatorEquals()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1 == date2, Is.True);
            Assert.That(date1 == date3, Is.False);
        }

        [Test]
        public void Date_OperatorNotEquals()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1 != date2, Is.False);
            Assert.That(date1 != date3, Is.True);
        }

        [Test]
        public void Date_OperatorLessThan()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1 < date2, Is.False);
            Assert.That(date1 < date3, Is.True);
        }

        [Test]
        public void Date_OperatorGreaterThan()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1 > date2, Is.False);
            Assert.That(date1 > date3, Is.False);
        }

        [Test]
        public void Date_OperatorLessThanOrEqual()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);

            Assert.That(date1 <= date2, Is.True);
            Assert.That(date1 <= date3, Is.True);
        }

        [Test]
        public void Date_OperatorGreaterThanOrEqual()
        {
            var date1 = new Date(2020, 1, 1);
            var date2 = new Date(2020, 1, 1);
            var date3 = new Date(2020, 1, 2);
            var date4 = new Date(2019, 12, 31);

            Assert.That(date1 >= date2, Is.True);
            Assert.That(date3 >= date1, Is.True);
            Assert.That(date1 >= date4, Is.True);
        }
    }
}
