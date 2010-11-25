namespace Nancy.Routing
{
    using System;

    public class RouteDescription : IEquatable<RouteDescription>
    {
        public Func<object, Response> Action { get; set; }

        public string Path { get; set;  }

        public string BaseRoute { get; set; }

        public bool Equals(RouteDescription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Path, Path) && Equals(other.Action, Action);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RouteDescription)) return false;
            return Equals((RouteDescription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Path.GetHashCode()*397) ^ Action.GetHashCode();
            }
        }
    }
}