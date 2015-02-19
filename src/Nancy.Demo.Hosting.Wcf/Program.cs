namespace Nancy.Demo.Hosting.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using Nancy.Hosting.Wcf;

    class Program
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:1234/Nancy/");

        static void Main()
        {
            using (CreateAndOpenWebServiceHost())
            {
                Console.WriteLine("Service is now running on: {0}", BaseUri);
                Console.ReadLine();
            }
        }

        private static WebServiceHost CreateAndOpenWebServiceHost()
        {
            var host = new WebServiceHost(
                new NancyWcfGenericService(new DefaultNancyBootstrapper()),
                BaseUri);

            host.AddServiceEndpoint(typeof(NancyWcfGenericService), new WebHttpBinding(), "");
            host.Open();

            return host;
        }
    }
}