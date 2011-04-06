namespace Nancy.Demo.Hosting.Self
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = parameters => {
                return View["staticview"];
            };

            Get["/hello/{id}"] = parameters =>
                                     {
                                         string id = parameters["id"];
                                         return string.Format("hello {0}", id);
                                     };
        }
    }
}