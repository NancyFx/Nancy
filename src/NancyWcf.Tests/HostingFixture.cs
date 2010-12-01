using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Nancy;
using Xunit;
using Xunit.Extensions;

namespace NancyWcf.Tests
{
    public class HostingFixture
    {
        public class TestModule : NancyModule
        {
           
            public TestModule()
            {
                Get["/rel"] = parameters =>
                {
                    return "This is the site route";
                };
            }
        }
        [Fact]
        public void Should_be_able_to_get_from_selfhost()
        {
            using (var host = new WebServiceHost(new NancyWcfGenericService(GetType().Assembly), new Uri("http://localhost/base/")))
            {
                host.AddServiceEndpoint(typeof (NancyWcfGenericService), new WebHttpBinding(), "");
                host.Open();

                var resp = new StreamReader(
                        WebRequest.Create("http://localhost/base/rel").GetResponse().GetResponseStream()
                    ).ReadToEnd();

                Assert.Equal("This is the site route", resp);
            }
        }
    }
}
