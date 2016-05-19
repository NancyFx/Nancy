namespace Nancy.Demo.Hosting.Owin
{
    using System.Collections.Generic;
    using Nancy.Owin;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get("", args => Root(args));
        }

        private object Root(dynamic o)
        {
            var env = GetOwinEnvironmentValue<IDictionary<string, object>>(this.Context.Items, NancyMiddleware.RequestEnvironmentKey);

            if (env == null)
            {
                return "Not running on owin host";
            }

            var requestMethod = GetOwinEnvironmentValue<string>(env, "owin.RequestMethod");
            var requestPath = GetOwinEnvironmentValue<string>(env, "owin.RequestPath");
            var owinVersion = GetOwinEnvironmentValue<string>(env, "owin.Version");
            var statusMessage = string.Format("You made a {0} request to {1} which runs on OWIN {2}.", requestMethod, requestPath, owinVersion);

            return View["Root", new Models.Index {StatusMessage = statusMessage}];
        }

        private static T GetOwinEnvironmentValue<T>(IDictionary<string, object> env, string name, T defaultValue = default(T))
        {
            object value;
            return env.TryGetValue(name, out value) && value is T ? (T)value : defaultValue;
        }

    }
}