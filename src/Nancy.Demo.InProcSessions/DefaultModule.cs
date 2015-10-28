namespace Nancy.Demo.InProcSessions
{
    public class DefaultModule : NancyModule
    {
        public DefaultModule()
        {
            this.Get["/"] = parameters =>
            {
                var currentSession = this.Context.Request.Session;
                var currentValue = (int?)currentSession["TestValue"];
                return
                    Response.AsText(
                        string.Format("Current session test value: {0}", currentValue.HasValue ? currentValue.Value.ToString() : "[null]"));
            };

            this.Get["/increment"] = parameters =>
            {
                var currentSession = this.Context.Request.Session;
                var currentValue = (int?)currentSession["TestValue"];

                currentSession["TestValue"] = currentValue.HasValue ? currentValue.Value + 1 : 0;

                return
                    Response.AsText(
                        string.Format("Current session test value after increment: {0}", currentSession["TestValue"]));
            };
        }
    }
}