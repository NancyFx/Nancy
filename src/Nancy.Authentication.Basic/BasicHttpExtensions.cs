using Nancy.Bootstrapper;

namespace Nancy.Authentication.Basic
{
	/// <summary>
	/// Some simple helpers give some nice authentication syntax in the modules.
	/// </summary>
	public static class BasicHttpExtensions
	{
		/// <summary>
		/// Module requires basic authentication
		/// </summary>
		/// <param name="module">Module to enable</param>
		/// <param name="configuration">Basic authentication configuration</param>
		public static void EnableBasicAuthentication(this NancyModule module, BasicAuthenticationConfiguration configuration)
		{
			BasicAuthentication.Enable(module, configuration);
		}

		/// <summary>
		/// Module requires basic authentication
		/// </summary>
		/// <param name="pipeline">Bootstrapper to enable</param>
		/// <param name="configuration">Basic authentication configuration</param>
		public static void EnableBasicAuthentication(this IPipelines pipeline, BasicAuthenticationConfiguration configuration)
		{
			BasicAuthentication.Enable(pipeline, configuration);
		}
	}
}
