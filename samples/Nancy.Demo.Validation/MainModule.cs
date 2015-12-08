namespace Nancy.Demo.Validation
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = x =>
            {
                return "<a href='/customers'>Customers</a><br><a href='/products'>Products</a>";
            };
        }
    }
}