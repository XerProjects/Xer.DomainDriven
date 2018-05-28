using System;
using System.Collections.Generic;
using System.Linq;

namespace Xer.DomainDriven
{
    public abstract partial class ValueObject<TSelf> : IEquatable<TSelf> where TSelf : class
    {
        protected struct HashCode
        {
            private readonly int _value;

            public HashCode(int value) => _value = value;   

            public static HashCode New => new HashCode(19);

            public HashCode Combine<T>(T obj)
            {
                int hashCode = EqualityComparer<T>.Default.GetHashCode(obj);
                return unchecked(new HashCode((_value * 31) + hashCode));
            }

            public static implicit operator int(HashCode hash) => hash._value;

            public override int GetHashCode() => _value;
        }
    }
}