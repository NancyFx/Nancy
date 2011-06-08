namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    /// <summary>
    /// Contains the result of an attempt to locate a view.
    /// </summary>
    public class ViewLocationResult : IEquatable<ViewLocationResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocationResult"/> class.
        /// </summary>
        /// <param name="location">The location of where the view was found.</param>
        /// <param name="name">The name of the view.</param>
        /// <param name="extension">The file extension of the located view.</param>
        /// <param name="contents">A <see cref="TextReader"/> that can be used to read the contents of the located view.</param>
        public ViewLocationResult(string location, string name, string extension, Func<TextReader> contents)
        {
            this.Location = location;
            this.Name = name;
            this.Extension = extension;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets a function that produces a reader for retrieving the contents of the view.
        /// </summary>
        /// <value>A <see cref="Func{T}"/> instance that can be used to produce a reader for retrieving the contents of the view.</value>
        public Func<TextReader> Contents { get; set; }

        /// <summary>
        /// Gets the extension of the view that was located.
        /// </summary>
        /// <value>A <see cref="string"/> containing the extension of the view that was located.</value>
        /// <remarks>The extension should not contain a leading dot.</remarks>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the location of where the view was found.
        /// </summary>
        /// <value>A <see cref="string"/> containing the location of the view.</value>
        public string Location { get; private set; }

        /// <summary>
        /// Gets the full name of the view that was found
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the view.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        /// <param name="other">An <see cref="ViewLocationResult"/> to compare with this instance.</param>
        public bool Equals(ViewLocationResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Extension, Extension) && Equals(other.Location, Location) && Equals(other.Name, Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, <see langword="false"/>.</returns>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (ViewLocationResult) && Equals((ViewLocationResult) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>A hash code for the current <see cref="ViewLocationResult"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = Extension.GetHashCode();
                result = (result*397) ^ Location.GetHashCode();
                result = (result*397) ^ Name.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(ViewLocationResult left, ViewLocationResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ViewLocationResult left, ViewLocationResult right)
        {
            return !Equals(left, right);
        }
    }
}