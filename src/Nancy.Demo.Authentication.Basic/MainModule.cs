namespace Nancy.Demo.Authentication.Basic
{
    public class MainModule : LegacyNancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => "<a href='/secure'>Enter</a>";
        }
    }
}