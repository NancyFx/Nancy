namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    public class RouteParameters : DynamicObject, IEquatable<RouteParameters>
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dictionary.TryGetValue(binder.Name, out result);
        }

        public object this[string name]
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