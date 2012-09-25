namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Default implementation of the <see cref="IRouteSegmentExtractor"/> interface.
    /// </summary>
    public class DefaultRouteSegmentExtractor : IRouteSegmentExtractor
    {
        /// <summary>
        /// Extracts the segments from the <paramref name="path"/>;
        /// </summary>
        /// <param name="path">The path that the segments should be extracted from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing the extracted segments.</returns>
        public IEnumerable<string> Extract(string path)
        {
            var currentSegment = string.Empty;
            var openingParenthesesCount = 0;

            for (var index = 0; index < path.Length; index++)
            {
                var token =
                    path[index];

                if (token.Equals('('))
                {
                    openingParenthesesCount++;
                }

                if (token.Equals(')'))
                {
                    openingParenthesesCount--;
                }

                if (!token.Equals('/') || openingParenthesesCount > 0)
                {
                    currentSegment += token;
                }

                if ((token.Equals('/') || index == path.Length - 1) && currentSegment.Length > 0 && openingParenthesesCount == 0)
                {
                    yield return currentSegment;
                    currentSegment = string.Empty;
                }
            }
        }
    }
}