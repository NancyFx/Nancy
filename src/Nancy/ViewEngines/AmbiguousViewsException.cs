namespace Nancy.ViewEngines
{
    using System;

    /// <summary>
    /// Thrown when multiple <see cref="ViewLocationResult"/> instances describe the exact same view.
    /// </summary>
    public class AmbiguousViewsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousViewsException"/> class.
        /// </summary>
        public AmbiguousViewsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousViewsException"/> class.
        /// </summary>
        /// <param name="message">The message that should be displayed with the exception.</param>
        public AmbiguousViewsException(string message)
            : base(message)
        {
        }
    }
}