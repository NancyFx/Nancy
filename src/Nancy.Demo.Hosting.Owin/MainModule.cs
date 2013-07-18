namespace Nancy.Demo.Hosting.Owin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Models;
    using Nancy.Hosting.Owin;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/moo"] = x => "moo";

            Get["/test3", true] = async (x,c) =>
                { 
                    await Task.Delay(500);

                    return "done";
                };

            Get["/test2", true] = async (x,c) =>
                {
                    var client = new HttpClient();

                    var res = await client.GetAsync("http://nancyfx.org");

                    var content = await res.Content.ReadAsStringAsync();

                    return content;
                };

            Get["/"] = x =>
                {
                    var model = new Index() { Name = "Boss Hawg" };

                    return View["Index", model];
                };

            Post["/"] = x =>
                {
                    var model = new Index() { Name = "Boss Hawg" };

                    model.Posted = this.Request.Form.posted.HasValue ? this.Request.Form.posted.Value : "Nothing :-(";

                    return View["Index", model];
                };

            Get["/fileupload"] = x =>
            {
                var model = new Index() { Name = "Boss Hawg" };

                return View["FileUpload", model];
            };

            Post["/fileupload"] = x =>
            {
                var model = new Index() { Name = "Boss Hawg" };

                var file = this.Request.Files.FirstOrDefault();
                string fileDetails = "None";

                if (file != null)
                {
                    fileDetails = string.Format("{3} - {0} ({1}) {2}bytes", file.Name, file.ContentType, file.Value.Length, file.Key);
                }

                model.Posted = fileDetails;

                return View["FileUpload", model];
            };

            Get["/owin"] = x =>
                               {
                                   var env = GetOwinEnvironmentValue<IDictionary<string, object>>(this.Context.Items, NancyOwinHost.RequestEnvironmentKey);
                                   if (env == null)
                                       return "Not running on owin host";

                                   var requestMethod = GetOwinEnvironmentValue<string>(env, "owin.RequestMethod");
                                   var requestPath = GetOwinEnvironmentValue<string>(env, "owin.RequestPath");
                                   var owinVersion = GetOwinEnvironmentValue<string>(env, "owin.Version");

                                   return string.Format("You made a {0} request to {1} which runs on owin {2}.", requestMethod, requestPath, owinVersion);
                               };

            Get["/error1"] = x => View["nope"];

        }

        private static T GetOwinEnvironmentValue<T>(IDictionary<string, object> env, string name, T defaultValue = default(T))
        {
            object value;
            return env.TryGetValue(name, out value) && value is T ? (T)value : defaultValue;
        }
    }
}