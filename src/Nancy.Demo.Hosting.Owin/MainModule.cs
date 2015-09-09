namespace Nancy.Demo.Hosting.Owin
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get[""] = Root;
        }

        private object Root(dynamic o)
        {
            return View["Root"];
        }
    }
}