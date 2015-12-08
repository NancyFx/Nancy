namespace Nancy.Demo.Hosting.Dnx
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = _ => "YO!";  
        }
    }
}
