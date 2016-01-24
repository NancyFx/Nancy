namespace Nancy.Tests.Functional.Modules
{
    public class SerializeTestModule : LegacyNancyModule
    {
        public SerializeTestModule()
        {
            Post["/serializedform"] = _ =>
            {
                var data = Request.Form.ToDictionary();

                return data;
            };

            Get["/serializedquerystring"] = _ =>
            {
                var data = Request.Query.ToDictionary();

                return data;
            };
        }
    }
}
