using System;
using System.Collections.Generic;
using System.Linq;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents a type that is distinguishable only by the state of its properties.
    /// </summary>
    /// <remarks>
    /// Two value objects that has the same properties are to be considered equal.
    /// Value objects should be implemented as immutable. Hence, all modifications to
    /// a value object should return a new instance with the updated properties.
    /// </remarks>
    /// <typeparam name="TSelf">Type of the derived class.</typeparam>
    public abstract partial class ValueObject<TSelf> : IEquatable<TSelf> where TSelf : class
    {
        /// <summary>
        /// Represents a hash code.
        /// </summary>
        protected struct HashCode
        {
            private readonly int _value;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="value">Hash code value.</param>
            public HashCode(int value) => _value = value;   

            /// <summary>
            /// A new HashCode that has an initial value.
            /// </summary>
            public static HashCode New => new HashCode(19);

            /// <summary>
            /// Combine current value with the hash code of the provided object.
            /// </summary>
            /// <param name="obj">Object to get the hash code to be combined.</param>
            /// <typeparam name="T">Type of object.</typeparam>
            /// <returns>New instance of HashCode that contains the combined hash code value.</returns>
            public HashCode Combine<T>(T obj)
            {
                int hashCode = EqualityComparer<T>.Default.GetHashCode(obj);
                return unchecked(new HashCode((_value * 31) + hashCode));
            }

            /// <summary>
            /// Implicit conversion from HashCode to int.
            /// </summary>
            /// <param name="hash"></param>
            public static implicit operator int(HashCode hash) => hash._value;

            /// <summary>
            /// Returns this HashCode instance's value.
            /// </summary>
            /// <returns>This HashCode instance's value.</returns>
            public override int GetHashCode() => _value;
        }
    }
}