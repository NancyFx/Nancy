namespace Nancy.Routing.Trie
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Match result for a matched route
    /// </summary>
    public class MatchResult : NodeData, IComparable<MatchResult>
    {
        private static readonly MatchResult noMatch = new MatchResult();

        private static readonly MatchResult[] noMatches = new MatchResult[] { };

        /// <summary>
        /// Gets or sets the captured parameters
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Gets the "no match" <see cref="MatchResult"/>
        /// </summary>
        public static MatchResult NoMatch
        {
            get
            {
                return noMatch;
            }
        }

        /// <summary>
        /// Gets the "no matches" <see cref="MatchResult"/> collection
        /// </summary>
        public static MatchResult[] NoMatches
        {
            get
            {
                return noMatches;
            }
        }

        public MatchResult(IDictionary<string, object> parameters)
        {
            this.Parameters = parameters;
        }

        public MatchResult()
            : this(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(MatchResult other)
        {
            // Length of the route takes precedence over score
            if (this.RouteLength < other.RouteLength)
            {
                return -1;
            }

            if (this.RouteLength > other.RouteLength)
            {
                return 1;
            }

            if (this.Score < other.Score)
            {
                return -1;
            }

            if (this.Score > other.Score)
            {
                return 1;
            }

            if (Equals(this.ModuleType, other.ModuleType))
            {
                if (this.RouteIndex < other.RouteIndex)
                {
                    return -1;
                }

                if (this.RouteIndex > other.RouteIndex)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}