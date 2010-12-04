namespace Nancy.Hosting.Wcf.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Nancy.Hosting.Wcf;
    using Nancy.Tests;
    using Xunit;

    public class HostingFixture
    {
        [Fact]
        public void Should_be_able_to_get_from_selfhost()
        {
            using (var host = new WebServiceHost(new NancyWcfGenericService(GetType().Assembly), new Uri("http://localhost:1234/base/")))
            {
                host.AddServiceEndpoint(typeof (NancyWcfGenericService), new WebHttpBinding(), "");
                host.Open();

                var reader =
                    new StreamReader(WebRequest.Create("http://localhost:1234/base/rel").GetResponse().GetResponseStream());

                var response = reader.ReadToEnd();

                response.ShouldEqual("This is the site route");
            }
        }
    }
}
