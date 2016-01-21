namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// Safe path configuration.
    /// </summary>
    public class StaticContentConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContentConfiguration"/> class.
        /// </summary>
        /// <param name="safePaths">StaticContent.</param>
        public StaticContentConfiguration(IEnumerable<string> safePaths)
        {
            this.SafePaths = safePaths;
        }

        /// <summary>
        /// Gets or sets the SafePaths.
        /// </summary>
        /// <value>The SafePaths.</value>
        public IEnumerable<string> SafePaths { get; private set; }
    }
}