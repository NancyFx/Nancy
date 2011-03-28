namespace Nancy.Demo.ModelBinding
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = x =>
                {
                    return "<a href='/events'>Events (default model binder)</a><br><a href='/customers'>Customers (custom model binder)</a><br>";
                };
        }
    }
}