namespace Nancy.Tests.Functional.Modules
{
    public class SerializeTestModule : NancyModule
    {
        public SerializeTestModule()
        {
            Post("/serializedform", args =>
            {
                var data = Request.Form.ToDictionary();

                return data;
            });

            Get("/serializedquerystring", args =>
            {
                var data = Request.Query.ToDictionary();

                return data;
            });
        }
    }
}
