namespace Nancy.SelfHosting.Demo
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = parameters => {
                return "Hello world";
            };
        }
    }
}