namespace Nancy.ViewEngines.Razor.Tests
{
    public abstract class GreetingViewBase : NancyRazorPage<dynamic>
    {
        public string Greet()
        {
            return "Hi, Nancy!";
        }
    }
}