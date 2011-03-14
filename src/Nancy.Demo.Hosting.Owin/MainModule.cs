namespace Nancy.Demo.Hosting.Owin
{
    using Models;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
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
        }
    }
}