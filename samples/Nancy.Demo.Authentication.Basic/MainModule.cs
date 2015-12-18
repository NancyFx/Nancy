namespace Nancy.Demo.Authentication.Basic
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => "<a href='/secure'>Enter</a>";
        }
    }
}