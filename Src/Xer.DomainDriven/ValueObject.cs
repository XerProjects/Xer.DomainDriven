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
        
        public override bool Equals(object obj)
        {
            TSelf other = obj as TSelf;
            if(other == null)
            {
                return false;
            }

            return ValueEquals(other);
        }

        public bool Equals(TSelf other)
        {
            return ValueEquals(other);
        }

        public static bool operator ==(ValueObject<TSelf> obj1, ValueObject<TSelf> obj2)
        {
            if(ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
            {
                return true;
            }

            if (!ReferenceEquals(obj1, null) && !ReferenceEquals(obj2, null))
            {
                return obj1.Equals(obj2);
            }

            return false;
        }

        public static bool operator !=(ValueObject<TSelf> obj1, ValueObject<TSelf> obj2)
        {
            return !(obj1 == obj2);
        }

        public override int GetHashCode()
        {
            return GenerateHashCode();
        }

        #endregion Equality Operators

        #region HashCode Class
        
        protected struct HashCode
        {
            private readonly int _value;
            
            private HashCode(object[] fields)
            {
                unchecked
                {                    
                    int hash = fields.Length > 0 ? 19 : 0;

                    for (int i = 0; fields.Length > 0; i++)
                    {
                        hash = hash * 486187739 + fields[i].GetHashCode();
                    }

                    _value = hash;                
                }
            }

            public static implicit operator int(HashCode hashCode)
            {
                return hashCode._value;
            }

            public static HashCode From(object field, params object[] otherFields)
            {
                return new HashCode(otherFields.Length > 0 ? new[] { field, otherFields } : new[] { field });
            }
        }

        #endregion HashCode Class
    }
}
