using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Data structure that represents a date (year, month, day).
    /// </summary>
    [Serializable]
    public struct Date : IEquatable<Date>
    {
        /// <summary>
        /// The current date.
        /// </summary>
        public static Date now => new Date(DateTime.Now);

        [SerializeField]
        int m_Year;

        /// <summary>
        /// The date year.
        /// </summary>
        public int year => m_Year;

        [SerializeField]
        int m_Month;

        /// <summary>
        /// The date month.
        /// </summary>
        public int month => m_Month;

        [SerializeField]
        int m_Day;

        /// <summary>
        /// The date day.
        /// </summary>
        public int day => m_Day;

        /// <summary>
        /// Constructs a <see cref="Date"/> from a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime"> The <see cref="DateTime"/> to construct from. </param>
        public Date(DateTime dateTime)
        {
            m_Year = dateTime.Year;
            m_Month = dateTime.Month;
            m_Day = dateTime.Day;
        }

        /// <summary>
        /// Constructs a <see cref="Date"/> from a year, month and day.
        /// </summary>
        /// <param name="year"> The year. </param>
        /// <param name="month"> The month. </param>
        /// <param name="day"> The day </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the year, month or day is out of range. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the day is out of range for the given year and month. </exception>
        public Date(int year, int month, int day)
        {
            m_Year = year;

            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), month, "Month must be between 1 and 12.");

            m_Month = month;

            var daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth)
                throw new ArgumentOutOfRangeException(nameof(day), day, $"Day must be between 1 and {daysInMonth}.");

            m_Day = day;
        }

        /// <summary>
        /// Converts a <see cref="Date"/> to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date"> The <see cref="Date"/> to convert. </param>
        /// <returns> The converted <see cref="DateTime"/>. </returns>
        public static implicit operator DateTime(Date date)
        {
            return new DateTime(date.year, date.month, date.day);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other"> The object to compare with the current object. </param>
        /// <returns> Whether the specified object is equal to the current object. </returns>
        public bool Equals(Date other)
        {
            return year == other.year && month == other.month && day == other.day;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object. </param>
        /// <returns> Whether the specified object is equal to the current object. </returns>
        public override bool Equals(object obj)
        {
            return obj is Date other && Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(year, month, day);
        }

        /// <summary>
        /// Determines whether two <see cref="Date"/>s are equal.
        /// </summary>
        /// <param name="left"> The first <see cref="Date"/> to compare. </param>
        /// <param name="right"> The second <see cref="Date"/> to compare. </param>
        /// <returns> Whether the two <see cref="Date"/>s are equal. </returns>
        public static bool operator ==(Date left, Date right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="Date"/>s are not equal.
        /// </summary>
        /// <param name="left"> The first <see cref="Date"/> to compare. </param>
        /// <param name="right"> The second <see cref="Date"/> to compare. </param>
        /// <returns> Whether the two <see cref="Date"/>s are not equal. </returns>
        public static bool operator !=(Date left, Date right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the left <see cref="Date"/> is less than the right <see cref="Date"/>.
        /// </summary>
        /// <param name="left"> The first <see cref="Date"/> to compare. </param>
        /// <param name="right"> The second <see cref="Date"/> to compare. </param>
        /// <returns> Whether the left <see cref="Date"/> is less than the right <see cref="Date"/>. </returns>
        public static bool operator <(Date left, Date right)
        {
            return DateTime.Compare(left, right) < 0;
        }

        /// <summary>
        /// Determines whether the left <see cref="Date"/> is greater than the right <see cref="Date"/>.
        /// </summary>
        /// <param name="left"> The first <see cref="Date"/> to compare. </param>
        /// <param name="right"> The second <see cref="Date"/> to compare. </param>
        /// <returns> Whether the left <see cref="Date"/> is greater than the right <see cref="Date"/>. </returns>
        public static bool operator >(Date left, Date right)
        {
            return DateTime.Compare(left, right) > 0;
        }

        /// <summary>
        /// Determines whether the left <see cref="Date"/> is less than or equal to the right <see cref="Date"/>.
        /// </summary>
        /// <param name="left"> The first <see cref="Date"/> to compare. </param>
        /// <param name="right"> The second <see cref="Date"/> to compare. </param>
        /// <returns> Whether the left <see cref="Date"/> is less than or equal to the right <see cref="Date"/>. </returns>
        public static bool operator <=(Date left, Date right)
        {
            return DateTime.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Determines whether the left <see cref="Date"/> is greater than or equal to the right <see cref="Date"/>.
        /// </summary>
        /// <param name="left"> The first <see cref="Date"/> to compare. </param>
        /// <param name="right"> The second <see cref="Date"/> to compare. </param>
        /// <returns> Whether the left <see cref="Date"/> is greater than or equal to the right <see cref="Date"/>. </returns>
        public static bool operator >=(Date left, Date right)
        {
            return DateTime.Compare(left, right) >= 0;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="Date"/>.
        /// </summary>
        /// <returns> A string that represents the current <see cref="Date"/>. </returns>
        public override string ToString()
        {
            return $"{year}-{month:D2}-{day:D2}";
        }
    }

    /// <summary>
    /// Data structure that represents a range of dates.
    /// </summary>
    [Serializable]
    public struct DateRange : IEquatable<DateRange>
    {
        [SerializeField]
        [Tooltip("The start date of the range.")]
        Date m_Start;

        [SerializeField]
        [Tooltip("The end date of the range.")]
        Date m_End;

        /// <summary>
        /// The start date of the range.
        /// </summary>
        public Date start => m_Start;

        /// <summary>
        /// The end date of the range.
        /// </summary>
        public Date end => m_End;

        /// <summary>
        /// Constructs a <see cref="DateRange"/> from a start and end date.
        /// </summary>
        /// <param name="start"> The start date. </param>
        /// <param name="end"> The end date. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the start date is greater than the end date. </exception>
        public DateRange(Date start, Date end)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException(nameof(start), start, "Start date must be less than or equal to the end date.");

            m_Start = start;
            m_End = end;
        }

        /// <summary>
        /// Constructs a <see cref="DateRange"/> from a start and end <see cref="DateTime"/>.
        /// </summary>
        /// <param name="start"> The start <see cref="DateTime"/>. </param>
        /// <param name="end"> The end <see cref="DateTime"/>. </param>
        public DateRange(DateTime start, DateTime end)
            : this(new Date(start), new Date(end))
        { }

        /// <summary>
        /// Determines whether the range contains a date.
        /// </summary>
        /// <param name="date"> The date to check. </param>
        /// <param name="includeStartAndEnd"> Whether to include the start and end dates. </param>
        /// <returns> Whether the range contains the date. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Date date, bool includeStartAndEnd = true)
        {
            return includeStartAndEnd ? date >= start && date <= end : date > start && date < end;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other"> The object to compare with the current object. </param>
        /// <returns> Whether the specified object is equal to the current object. </returns>
        public bool Equals(DateRange other)
        {
            return start.Equals(other.start) && end.Equals(other.end);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object. </param>
        /// <returns> Whether the specified object is equal to the current object. </returns>
        public override bool Equals(object obj)
        {
            return obj is DateRange other && Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(start, end);
        }

        /// <summary>
        /// Determines whether two <see cref="DateRange"/>s are equal.
        /// </summary>
        /// <param name="left"> The first <see cref="DateRange"/> to compare. </param>
        /// <param name="right"> The second <see cref="DateRange"/> to compare. </param>
        /// <returns> Whether the two <see cref="DateRange"/>s are equal. </returns>
        public static bool operator ==(DateRange left, DateRange right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="DateRange"/>s are not equal.
        /// </summary>
        /// <param name="left"> The first <see cref="DateRange"/> to compare. </param>
        /// <param name="right"> The second <see cref="DateRange"/> to compare. </param>
        /// <returns> Whether the two <see cref="DateRange"/>s are not equal. </returns>
        public static bool operator !=(DateRange left, DateRange right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Try to parse a string into a <see cref="DateRange"/>.
        /// </summary>
        /// <param name="value"> The string to parse. </param>
        /// <param name="dateRange"> The parsed <see cref="DateRange"/>. </param>
        /// <returns> Whether the string was successfully parsed. </returns>
        public static bool TryParse(string value, out DateRange dateRange)
        {
            dateRange = default;

            if (string.IsNullOrEmpty(value))
                return false;

            var parts = value.Split(',');
            if (parts.Length != 2)
                return false;

            if (!DateTime.TryParse(parts[0], out var start) || !DateTime.TryParse(parts[1], out var end))
                return false;

            if (start > end)
                return false;

            dateRange = new DateRange(new Date(start), new Date(end));
            return true;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="DateRange"/>.
        /// </summary>
        /// <returns> A string that represents the current <see cref="DateRange"/>. </returns>
        public override string ToString()
        {
            return $"{start},{end}";
        }
    }
}
