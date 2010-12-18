namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq.Expressions;
    using Microsoft.CSharp.RuntimeBinder;

    public class DynamicDictionary : DynamicObject, IEquatable<DynamicDictionary>
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
        	this[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dictionary.TryGetValue(binder.Name, out result);
        }

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return dictionary.Keys;
		}

        public dynamic this[string name]
        {
            get { return dictionary[name]; }
            set { dictionary[name] = value is DynamicDictionaryValue ? value : new DynamicDictionaryValue(value); }
        }

        /// <summary>
        /// Indicates whether the current <see cref="DynamicDictionary"/> is equal to another object of the same type.
        /// </summary>
        /// <returns><see langword="true"/> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        /// <param name="other">An <see cref="DynamicDictionary"/> instance to compare with this instance.</param>
        public bool Equals(DynamicDictionary other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(other.dictionary, this.dictionary);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof (DynamicDictionary) && this.Equals((DynamicDictionary) obj);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="DynamicDictionary"/>.
        /// </summary>
        /// <returns> A hash code for this <see cref="DynamicDictionary"/>, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return (dictionary != null ? dictionary.GetHashCode() : 0);
        }

        internal class DynamicDictionaryValue : DynamicObject
        {
            private readonly object value;

            public DynamicDictionaryValue(object value)
            {
                this.value = value;
            }

            public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
            {
                object resultOfCast;
                result = null;

                if (binder.Operation != ExpressionType.Equal)
                {
                    return false;
                }

                var convert =
                    Binder.Convert(CSharpBinderFlags.None, arg.GetType(), typeof(DynamicDictionaryValue));

                if (!TryConvert((ConvertBinder)convert, out resultOfCast))
                {
                    return false;
                }

                result = (resultOfCast == null) ?
                    Equals(arg, resultOfCast) :
                    resultOfCast.Equals(arg);

                return true;
            }

            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                result = null;

                if (value == null)
                {
                    return true;
                }

                var binderType = binder.Type;
                if (binderType == typeof(String))
                {
                    result = Convert.ToString(value);
                    return true;
                }

                if (binderType == typeof(Guid) || binderType == typeof(Guid?))
                {
                    Guid guid;
                    if (Guid.TryParse(Convert.ToString(value), out guid))
                    {
                        result = guid;
                        return true;
                    }
                }
                else if (binderType == typeof(TimeSpan) || binderType == typeof(TimeSpan?))
                {
                    TimeSpan timespan;
                    if (TimeSpan.TryParse(Convert.ToString(value), out timespan))
                    {
                        result = timespan;
                        return true;
                    }
                }
                else
                {
                    if (binderType.IsGenericType && binderType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        binderType = binderType.GetGenericArguments()[0];
                    }

                    var typeCode = Type.GetTypeCode(binderType);

                    if (typeCode == TypeCode.Object) // something went wrong here
                    {
                        return false;
                    }

                    result = Convert.ChangeType(value, typeCode);

                    return true;
                }
                return base.TryConvert(binder, out result);
            }

        }
    }
}