namespace Nancy.ViewEngines
{
    using System;

    /// <summary>
    /// Thrown when multiple <see cref="ViewLocationResult"/> instances describe the exact same view.
    /// </summary>
    public class AmbiguousViewsException : Exception
    {
    }
}