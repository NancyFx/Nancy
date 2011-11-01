using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Tests.Functional.Modules
{
    public class JsonpTestModule : NancyModule
    {
        public JsonpTestModule() : base("/test")
        {
            Get["/string"] = x => "Normal Response";
            Get["/json"] = x => Response.AsJson(true);
        }
    }
}
