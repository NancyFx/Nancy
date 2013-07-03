using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Tests.Functional.Modules
{
    public class SerializeTestModule : NancyModule
    {
        public SerializeTestModule()
        {
            Post["/serializedform"] = _ =>
            {
                var data = Request.Form.Serializable();

                return data;
            };

            Get["/serializedquerystring"] = _ =>
            {
                var data = Request.Query.Serializable();

                return data;
            };
        }
    }
}
