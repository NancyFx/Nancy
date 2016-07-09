namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="RouteConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class RouteConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="RouteConfiguration"/>.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="disableMethodNotAllowedResponses"><see langword="true"/>If 405 responses are allowed, otherwise <see langword="false"/>.</param>
        /// <param name="explicitHeadRouting"><see langword="true"/>If explicit HEAD route requests are allowed, otherwise <see langword="false"/>.</param>
        public static void Routing(this INancyEnvironment environment, bool? disableMethodNotAllowedResponses = false, bool? explicitHeadRouting = false)
        {
            environment.AddValue(new RouteConfiguration(
                disableMethodNotAllowedResponses: disableMethodNotAllowedResponses ?? RouteConfiguration.Default.DisableMethodNotAllowedResponses,
                explicitHeadRouting: explicitHeadRouting ?? RouteConfiguration.Default.ExplicitHeadRouting));
        }
    }
}