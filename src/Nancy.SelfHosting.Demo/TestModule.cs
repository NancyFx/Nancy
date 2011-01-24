namespace Nancy.SelfHosting.Demo
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = request =>
                           {
                               return "Hello world";
                           };
        }
    }
}