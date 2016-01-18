namespace Nancy
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Safe path configuration.
    /// </summary>
    public class SafePathConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Nancy.SafePathConfiguration"/> class.
        /// </summary>
        /// <param name="paths">AddSafePaths.</param>
        public SafePathConfiguration(IEnumerable<string> paths)
        {
            this.Paths = paths;
        }

        /// <summary>
        /// Gets or sets the paths.
        /// </summary>
        /// <value>The paths.</value>
        public IEnumerable<string> Paths { get; private set; }
    }
}