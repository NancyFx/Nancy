namespace Nancy
{
    /// <summary>
    /// Configuration for view rendering.
    /// </summary>
    public class ViewConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="ViewConfiguration"/> class.
        /// </summary>
        public static readonly ViewConfiguration Default = new ViewConfiguration(
            runtimeViewDiscovery: false,
            runtimeViewUpdates: false);

        private ViewConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewConfiguration"/> class.
        /// </summary>
        /// <param name="runtimeViewDiscovery">Determines if views can be discovered during runtime.</param>
        /// <param name="runtimeViewUpdates">Determines if views can be updated during runtime.</param>
        public ViewConfiguration(bool runtimeViewDiscovery = false, bool runtimeViewUpdates = false)
        {
            this.RuntimeViewDiscovery = runtimeViewDiscovery;
            this.RuntimeViewUpdates = runtimeViewUpdates;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable runtime view discovery
        /// </summary>
        /// <value><see langword="true"/> if views can be discovered during runtime, otherwise <see langword="false"/>.</value>
        public bool RuntimeViewDiscovery { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to allow runtime changes of views.
        /// </summary>
        /// <value><see langword="true"/> if views can be updated during runtime, otherwise <see langword="false"/>.</value>
        public bool RuntimeViewUpdates { get; private set; }
    }
}
