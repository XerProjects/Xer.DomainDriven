using System;

namespace Xer.DomainDriven
{
    public abstract class ValueObject<TSelf> : IEquatable<TSelf> where TSelf : class
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

            if (!ReferenceEquals(obj1, null) && !ReferenceEquals(obj2, null))
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

        #region HashCode Class
        
        /// <summary>
        /// Represents a hash code.
        /// </summary>
        protected struct HashCode
        {
            private readonly int _value;
            
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="fields">Value object fields.</param>
            private HashCode(object[] fields)
            {
                if (fields == null)
                {
                    throw new ArgumentNullException(nameof(fields));
                }

                if (fields.Length == 0)
                    _value = 0;

                unchecked
                {                    
                    int hash = 19;

                    for (int i = 0; fields.Length > i; i++)
                    {
                        hash = hash * 486187739 + fields[i]?.GetHashCode() ?? throw new ArgumentException("Cannot pass null as field.", nameof(fields));
                    }
                    
                    _value = hash;                
                }
            }

            /// <summary>
            /// Implicit convenrsion from HashCode to int.
            /// </summary>
            /// <param name="hashCode">HashCode.</param>
            public static implicit operator int(HashCode hashCode)
            {
                return hashCode._value;
            }

            /// <summary>
            /// Create HashCode derived from the list of fields.
            /// </summary>
            /// <param name="fields">Value object fields.</param>
            /// <returns>HashCode instance.</returns>
            public static HashCode From(params object[] fields)
            {
                return new HashCode(fields);
            }
        }

        #endregion HashCode Class
    }
}
