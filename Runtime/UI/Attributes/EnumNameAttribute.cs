using System;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Place this attribute on a constant string field to generate a method to return a string representation of the enum value.
    /// <para/>
    /// The constant string will be used as a prefix in the returned value from the generated method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    class EnumNameAttribute : Attribute
    {
        /// <summary>
        /// The type of the enum you will be able to use as argument in the generated method.
        /// </summary>
        public Type enumType { get; }

        /// <summary>
        /// The name of the generated method.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Place this attribute on a constant string field to generate a method to return a string representation of the enum value.
        /// <para/>
        /// The constant string will be used as a prefix in the returned value from the generated method.
        /// </summary>
        /// <param name="Name"> The name of the generated method. </param>
        /// <param name="enumType"> The type of the enum you will be able to use as argument in the generated method. </param>
        public EnumNameAttribute(string Name, Type enumType)
        {
            this.enumType = enumType;
        }
    }
}
