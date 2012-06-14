namespace Nancy.Testing.Experiments
{
    using Nancy;

    public class Home : NancyModule
    {
        public Home()
        {
            Get["/"] = parameters => {
                return View["index"];
            };
        }
    }
}