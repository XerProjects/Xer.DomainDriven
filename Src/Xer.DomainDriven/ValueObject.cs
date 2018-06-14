using System;

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
        #region Protected Methods
            
        /// <summary>
        /// Generate a HashCode by passing all the value object's fields to the HashCode's constructor.
        /// </summary>
        /// <returns>Instance of HashCode.</returns>
        protected abstract HashCode GenerateHashCode();

        /// <summary>
        /// Compare equality by value.
        /// </summary>
        /// <param name="other">Other instance.</param>
        /// <returns>True if objects should be treated as equal by value. Otherwise, false.</returns>
        protected abstract bool ValueEquals(TSelf other);

        #endregion Protected Methods

        #region Equality Operators
        
        /// <summary>
        /// Equals method.
        /// </summary>
        /// <param name="obj">Other object.</param>
        /// <returns>True, if objects are determined as equal. Otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as TSelf);
        }

        /// <summary>
        /// Equals method.
        /// </summary>
        /// <param name="other">Other object.</param>
        /// <returns>True, if objects are determined as equal. Otherwise, false.</returns>
        public bool Equals(TSelf other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;
                
            if (this.GetType() != other.GetType())
                return false;

            return ValueEquals(other);
        }
        
        /// <summary>
        /// Equals operator.
        /// </summary>
        /// <param name="obj1">First value object.</param>
        /// <param name="obj2">Second value object.</param>
        /// <returns>True, if value objects are determined as equal. Otherwise, false.</returns>
        public static bool operator ==(ValueObject<TSelf> obj1, ValueObject<TSelf> obj2)
        {
            if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
            {
                return true;
            }

            if (!ReferenceEquals(obj1, null))
            {
                return obj1.Equals(obj2);
            }
            
            return false;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="obj1">First value object.</param>
        /// <param name="obj2">Second value object.</param>
        /// <returns>True, if value objects are determined as NOT equal. Otherwise, false.</returns>
        public static bool operator !=(ValueObject<TSelf> obj1, ValueObject<TSelf> obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GenerateHashCode();
        }

        #endregion Equality Operators
    }
}
