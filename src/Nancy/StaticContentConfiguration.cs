namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// Static content configuration.
    /// </summary>
    public class StaticContentConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContentConfiguration"/> class.
        /// </summary>
        /// <param name="safePaths">A set of safe paths to retrieve static content from</param>
        public StaticContentConfiguration(IEnumerable<string> safePaths)
        {
            this.SafePaths = safePaths;
        }

        /// <summary>
        /// Gets the safe paths to retrieve static content from.
        /// </summary>
        /// <value>Safe paths to retrieve static content from</value>
        public IEnumerable<string> SafePaths { get; private set; }
    }
}