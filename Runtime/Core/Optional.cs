using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A struct that can be used to represent an optional value.
    /// </summary>
    /// <typeparam name="T"> The type of the optional value </typeparam>
    [Serializable]
    public struct Optional<T> : IEquatable<Optional<T>>
    {
        /// <summary>
        /// Whether the value is set.
        /// </summary>
        [SerializeField]
        bool isSet;

        /// <summary>
        /// The current value.
        /// </summary>
        /// <remarks>
        /// This field is serialized even if <see cref="isSet"/> is false.
        /// Please use <see cref="IsSet"/> to check if the value is set.
        /// </remarks>
        [SerializeField]
        T value;

        /// <summary>
        /// Whether the value is set.
        /// </summary>
        public bool IsSet => isSet;

        /// <summary>
        /// The current value.
        /// </summary>
        /// <remarks>
        /// This property will throw an exception if <see cref="IsSet"/> is false.
        /// Please use <see cref="IsSet"/> to check if the value is set.
        /// </remarks>
        public T Value => value;

        /// <summary>
        /// Constructs an <see cref="Optional{T}"/> with the given value.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <remarks>
        /// The <see cref="IsSet"/> property will become true.
        /// </remarks>
        public Optional(T value)
        {
            this.value = value;
            isSet = true;
        }

        /// <summary>
        /// An <see cref="Optional{T}"/> with no value set.
        /// </summary>
        public static Optional<T> none => default;

        /// <summary>
        /// Automatically converts a value to an <see cref="Optional{T}"/>.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An <see cref="Optional{T}"/> with the given value. </returns>
        /// <remarks>
        /// The <see cref="IsSet"/> property will become true.
        /// </remarks>
        public static implicit operator Optional<T>(T value) => new (value);

        /// <summary>
        /// Determines whether two <see cref="Optional{T}"/>s are equal.
        /// </summary>
        /// <param name="other"> The other <see cref="Optional{T}"/> to compare. </param>
        /// <returns> Whether the two <see cref="Optional{T}"/>s are equal. </returns>
        public bool Equals(Optional<T> other)
        {
            return isSet == other.isSet && EqualityComparer<T>.Default.Equals(value, other.value);
        }

        /// <summary>
        /// Determines whether two <see cref="Optional{T}"/>s are equal.
        /// </summary>
        /// <param name="obj"> The other object to compare. </param>
        /// <returns> Whether the two <see cref="Optional{T}"/>s are equal. </returns>
        public override bool Equals(object obj)
        {
            return obj is Optional<T> other && Equals(other);
        }

        /// <summary>
        /// Gets the hash code of the <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns> The hash code of the <see cref="Optional{T}"/>. </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(IsSet, Value);
        }

        /// <summary>
        /// Determines whether two <see cref="Optional{T}"/>s are equal.
        /// </summary>
        /// <param name="left"> The first <see cref="Optional{T}"/> to compare. </param>
        /// <param name="right"> The second <see cref="Optional{T}"/> to compare. </param>
        /// <returns> Whether the two <see cref="Optional{T}"/>s are equal. </returns>
        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="Optional{T}"/>s are not equal.
        /// </summary>
        /// <param name="left"> The first <see cref="Optional{T}"/> to compare. </param>
        /// <param name="right"> The second <see cref="Optional{T}"/> to compare. </param>
        /// <returns> Whether the two <see cref="Optional{T}"/>s are not equal. </returns>
        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// A struct that can be used to represent an optional enum value.
    /// </summary>
    /// <typeparam name="T"> The enum type. </typeparam>
    [Serializable]
    public struct OptionalEnum<T> : IEquatable<OptionalEnum<T>>
        where T : Enum
    {
        /// <summary>
        /// Whether the value is set.
        /// </summary>
        [SerializeField]
        bool isSet;

        /// <summary>
        /// The current value.
        /// </summary>
        /// <remarks>
        /// This field is serialized even if <see cref="isSet"/> is false.
        /// Please use <see cref="IsSet"/> to check if the value is set.
        /// </remarks>
        [SerializeField]
        T value;

        /// <summary>
        /// Whether the value is set.
        /// </summary>
        public bool IsSet => isSet;

        /// <summary>
        /// The current value.
        /// </summary>
        /// <remarks>
        /// This field is serialized even if <see cref="isSet"/> is false.
        /// Please use <see cref="IsSet"/> to check if the value is set.
        /// </remarks>
        public T Value => value;

        /// <summary>
        /// Constructs an <see cref="OptionalEnum{T}"/> with the given value.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <remarks>
        /// The <see cref="IsSet"/> property will become true.
        /// </remarks>
        public OptionalEnum(T value)
        {
            this.value = value;
            isSet = true;
        }

        /// <summary>
        /// An <see cref="OptionalEnum{T}"/> with no value set.
        /// </summary>
        public static OptionalEnum<T> none => default;

        /// <summary>
        /// Automatically converts a value to an <see cref="OptionalEnum{T}"/>.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An <see cref="OptionalEnum{T}"/> with the given value. </returns>
        /// <remarks>
        /// The <see cref="IsSet"/> property will become true.
        /// </remarks>
        public static implicit operator OptionalEnum<T>(T value) => new (value);

        /// <summary>
        /// Determines whether two <see cref="OptionalEnum{T}"/>s are equal.
        /// </summary>
        /// <param name="other"> The other <see cref="OptionalEnum{T}"/> to compare. </param>
        /// <returns> Whether the two <see cref="OptionalEnum{T}"/>s are equal. </returns>
        public bool Equals(OptionalEnum<T> other)
        {
            return isSet == other.isSet && value.Equals(other.value);
        }

        /// <summary>
        /// Determines whether two <see cref="OptionalEnum{T}"/>s are equal.
        /// </summary>
        /// <param name="obj"> The other object to compare. </param>
        /// <returns> Whether the two <see cref="OptionalEnum{T}"/>s are equal. </returns>
        public override bool Equals(object obj)
        {
            return obj is OptionalEnum<T> other && Equals(other);
        }

        /// <summary>
        /// Gets the hash code of the <see cref="OptionalEnum{T}"/>.
        /// </summary>
        /// <returns> The hash code of the <see cref="OptionalEnum{T}"/>. </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(IsSet, Value);
        }

        /// <summary>
        /// Determines whether two <see cref="OptionalEnum{T}"/>s are equal.
        /// </summary>
        /// <param name="left"> The first <see cref="OptionalEnum{T}"/> to compare. </param>
        /// <param name="right"> The second <see cref="OptionalEnum{T}"/> to compare. </param>
        /// <returns> Whether the two <see cref="OptionalEnum{T}"/>s are equal. </returns>
        public static bool operator ==(OptionalEnum<T> left, OptionalEnum<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="OptionalEnum{T}"/>s are not equal.
        /// </summary>
        /// <param name="left"> The first <see cref="OptionalEnum{T}"/> to compare. </param>
        /// <param name="right"> The second <see cref="OptionalEnum{T}"/> to compare. </param>
        /// <returns> Whether the two <see cref="OptionalEnum{T}"/>s are not equal. </returns>
        public static bool operator !=(OptionalEnum<T> left, OptionalEnum<T> right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// A UI-Toolkit field that can be used to edit an <see cref="Optional{T}"/>.
    /// </summary>
    /// <typeparam name="T"> The type of the value. </typeparam>
    public class OptionalField<T> : BaseField<Optional<T>>
    {
        /// <summary>
        /// The USS class name of this element.
        /// </summary>
        public new const string ussClassName = "unity-optional-field";

        /// <summary>
        /// Constructs an <see cref="OptionalField{T}"/> with the given label and visual input.
        /// </summary>
        /// <param name="label"> The label of the field. </param>
        /// <param name="visualInput"> The visual input of the field. </param>
        public OptionalField(string label, VisualElement visualInput)
            : base(label, visualInput)
        {
            AddToClassList(ussClassName);
        }
    }

    /// <summary>
    /// A UI-Toolkit field that can be used to edit an <see cref="OptionalEnum{T}"/>.
    /// </summary>
    /// <typeparam name="T"> The enum type. </typeparam>
    public class OptionalEnumField<T> : BaseField<OptionalEnum<T>>
        where T : Enum
    {
        /// <summary>
        /// The USS class name of this element.
        /// </summary>
        public new const string ussClassName = "unity-optional-field";

        /// <summary>
        /// Constructs an <see cref="OptionalEnumField{T}"/> with the given label and visual input.
        /// </summary>
        /// <param name="label"> The label of the field. </param>
        /// <param name="visualInput"> The visual input of the field. </param>
        public OptionalEnumField(string label, VisualElement visualInput)
            : base(label, visualInput)
        {
            AddToClassList(ussClassName);
        }
    }
}
