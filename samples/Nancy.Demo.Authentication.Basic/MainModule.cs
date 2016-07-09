namespace Nancy.Demo.Authentication.Basic
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get("/", args => "<a href='/secure'>Enter</a>");
        }
    }
}