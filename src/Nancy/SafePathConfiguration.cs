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
        /// A default instance of the <see cref="SafePathConfiguration"/> class 
        /// </summary>
        public static readonly SafePathConfiguration Default = new SafePathConfiguration(
                                                                   Enumerable.Empty<string>());

        /// <summary>
        /// Initializes a new instance of the <see cref="Nancy.SafePathConfiguration"/> class.
        /// </summary>
        /// <param name="paths">Paths.</param>
        public SafePathConfiguration(IEnumerable<string> paths)
        {
            this.Paths = paths ?? Default.Paths;
        }

        /// <summary>
        /// Gets or sets the paths.
        /// </summary>
        /// <value>The paths.</value>
        public IEnumerable<string> Paths{ get; set; }
    }

}

