namespace Nancy.Routing
{
    using System;

    public class RouteDescription : IEquatable<RouteDescription>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Func<object, Response> Action { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Func<bool> Condition { get; set; }

        /// <summary>
        /// Gets or sets the path that this route will match.
        /// </summary>
        /// <value>A <see cref="string"/> containing the path of the route.</value>
        /// <remarks>This will be the module-qualified path, i.e a combination of the <see cref="NancyModule.ModulePath"/>, of the module that the route belogs to, and the registered path.</remarks>
        public string Path { get; set; }

        public string Method { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public NancyModule Module { get; set; }

        /// <summary>
        /// Indicates whether the current <see cref="RouteDescription"/> is equal to another <see cref="RouteDescription"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the current <see cref="RouteDescription"/> is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        /// <param name="other">An <see cref="RouteDescription"/> to compare with this <see cref="RouteDescription"/>.</param>
        public bool Equals(RouteDescription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Module, Module) && Equals(other.Path, Path) && Equals(other.Condition, Condition) && Equals(other.Action, Action);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, <see langword="false"/>.</returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(RouteDescription) && Equals((RouteDescription)obj);
        }

        /// <summary>
        /// Gets the hash code of the <see cref="RouteDescription"/> instance.
        /// </summary>
        /// <returns>A hash code for the current <see cref="RouteDescription"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = (this.Module != null ? this.Module.GetHashCode() : 0);
                result = (result * 397) ^ (this.Path != null ? this.Path.GetHashCode() : 0);
                result = (result * 397) ^ (this.Condition != null ? this.Condition.GetHashCode() : 0);
                result = (result * 397) ^ (this.Action != null ? this.Action.GetHashCode() : 0);
                return result;
            }
        }
    }
}
