namespace Nancy.Demo.ModelBinding
{
    public class MainModule : LegacyNancyModule
    {
        public MainModule()
        {
            Get["/"] = x =>
                {
                    return "<a href='/events'>Events (default model binder)</a><br><a href='/customers'>Customers (custom model binder)</a><br><a href='/bindjson'>Users (JSON)</a></a><br><a href='/bindxml'>Users (XML)</a><br>";
                };
        }
    }
}