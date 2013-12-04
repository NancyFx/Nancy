namespace Demo.Embedded
{
    using System;

    using Nancy.Hosting.Self;

    public static class Program
    {
        private const string BaseUri = "http://localhost:8080/";

        public static void Main()
        {
            using (var host = new NancyHost(new Uri(BaseUri)))
            {
                host.Start();
                Console.WriteLine("Nancy listening on " + BaseUri);
                Console.ReadLine();
            }
        }
    }
}
