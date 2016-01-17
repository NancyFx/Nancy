namespace Nancy
{
    /// <summary>
    /// Configuration for the default routing.
    /// </summary>
    public class RouteConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="ViewConfiguration"/> class.
        /// </summary>
        public static readonly RouteConfiguration Default = new RouteConfiguration(
            disableMethodNotAllowedResponses: false,
            explicitHeadRouting: false);

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteConfiguration"/> class.
        /// </summary>
        /// <param name="disableMethodNotAllowedResponses">Determins is 405 responses are allowed.</param>
        /// <param name="explicitHeadRouting">Enabled support for explicit HEAD route declarations.</param>
        public RouteConfiguration(bool disableMethodNotAllowedResponses = false, bool explicitHeadRouting = false)
        {
            this.DisableMethodNotAllowedResponses = disableMethodNotAllowedResponses;
            this.ExplicitHeadRouting = explicitHeadRouting;
        }

        /// <summary>
        /// Gets a value indicating whether or not to respond with 405 responses.
        /// </summary>
        /// <value><see langword="true"/>If 405 responses are allowed, otherwise <see langword="false"/>.</value>
        public bool DisableMethodNotAllowedResponses { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to route HEAD requests explicitly.
        /// </summary>
        /// <value><see langword="true"/>If explicit HEAD route requests are allowed, otherwise <see langword="false"/>.</value>
        public bool ExplicitHeadRouting { get; private set; }
    }
}
