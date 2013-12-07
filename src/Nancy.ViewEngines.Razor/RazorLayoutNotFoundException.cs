namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a layout is not found.
    /// </summary>
    public class RazorLayoutNotFoundException : Exception
    {
        /// <summary>
        /// initializes a new instance of the <see cref="RazorLayoutNotFoundException"/>
        /// </summary>
        /// <param name="layoutName">The name of the layout that could not be found.</param>
        /// <param name="innerViewLocationResult">The location of the view or layout that the missing layout should wrap.</param>
        public RazorLayoutNotFoundException(string layoutName, ViewLocationResult innerViewLocationResult)
            : base(String.Format("The view '{0} {1}' referenced layout '{2}' but it could not be found.", innerViewLocationResult.Location, innerViewLocationResult.Name, layoutName))
        {
            
        }

        protected RazorLayoutNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
