namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for extracting the individual segments from a route path.
    /// </summary>
    public interface IRouteSegmentExtractor
    {
        /// <summary>
        /// Extracts the segments from the <paramref name="path"/>;
        /// </summary>
        /// <param name="path">The path that the segments should be extracted from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing the extracted segments.</returns>
        IEnumerable<string> Extract(string path);
    }
}