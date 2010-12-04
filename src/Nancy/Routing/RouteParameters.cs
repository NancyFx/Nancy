using System.Linq.Expressions;
using Microsoft.CSharp.RuntimeBinder;

namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    public class RouteParameters : DynamicObject, IEquatable<RouteParameters>
    {
		internal class DynamicRouteParameter : DynamicObject
		{
			private readonly object value;

			public DynamicRouteParameter(object value)
			{
				this.value = value;
			}

			public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
			{
				object resultOfCast;
				result = null;

				if (binder.Operation != ExpressionType.Equal)
					return false;

				var convert = Binder.Convert(CSharpBinderFlags.None, arg.GetType(), typeof(DynamicRouteParameter));
				if (false == TryConvert((ConvertBinder)convert, out resultOfCast))
				{
					return false;
				}

				result = resultOfCast == null
							? Equals(arg, resultOfCast)
							: resultOfCast.Equals(arg);
				return true;
			}
			public override bool TryConvert(ConvertBinder binder, out object result)
			{
				result = null;
				if (value == null)
					return true;
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
						binderType = binderType.GetGenericArguments()[0];
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

        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
			dictionary[binder.Name] = value is DynamicRouteParameter ? value : new DynamicRouteParameter(value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dictionary.TryGetValue(binder.Name, out result);
        }

        public dynamic this[string name]
        {
            get { return dictionary[name]; }
            set { dictionary[name] = value; }
        }

        public bool Equals(RouteParameters other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.dictionary, dictionary);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RouteParameters)) return false;
            return Equals((RouteParameters) obj);
        }

        public override int GetHashCode()
        {
            return (dictionary != null ? dictionary.GetHashCode() : 0);
        }
    }
}