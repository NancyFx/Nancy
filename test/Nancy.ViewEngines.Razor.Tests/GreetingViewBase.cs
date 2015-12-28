namespace Nancy.ViewEngines.Razor.Tests
{
    public abstract class GreetingViewBase : NancyRazorViewBase
    {
        public string Greet()
        {
            return "Hi, Nancy!";
        }
    }
}