namespace Nancy.Routing
{
    using System;

    public class RouteDescription : IEquatable<RouteDescription>
    {
        public Func<object, Response> Action { get; set; }

        public string Path { get; set;  }

        public string ModulePath { get; set; }

        public bool Equals(RouteDescription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Action, Action) && Equals(other.Path, Path) && Equals(other.ModulePath, this.ModulePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == typeof (RouteDescription) && Equals((RouteDescription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Action != null ? Action.GetHashCode() : 0);
                result = (result*397) ^ (Path != null ? Path.GetHashCode() : 0);
                result = (result*397) ^ (this.ModulePath != null ? this.ModulePath.GetHashCode() : 0);
                return result;
            }
        }
    }
}