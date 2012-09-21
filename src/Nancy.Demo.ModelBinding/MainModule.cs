namespace Nancy.Demo.ModelBinding
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = x =>
                {
                    return "<a href='/events'>Events (default model binder)</a><br><a href='/events/bindto'>Events - BindTo (bind to existing instance with default binder)</a><br><a href='/customers'>Customers (custom model binder)</a><br><a href='/customers/bindto'>Customer - BindTo (bind to existing instance with custom binder)</a><br><a href='/bindjson'>Users (JSON)</a></a><br><a href='/bindxml'>Users (XML)</a><br>";
                };
        }
    }
}