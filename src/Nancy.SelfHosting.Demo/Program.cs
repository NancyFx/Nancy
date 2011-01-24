namespace Nancy.SelfHosting.Demo
{
    using System;
    using Hosting.SelfHosting;

    class Program
    {
        static void Main(string[] args)
        {
            var nancyHost = new NancyHost(new Uri("http://localhost:8888/nancy/"));
            nancyHost.Start();
            Console.WriteLine("Nancy now listening - navigate to http://localhost:8888/nancy/. Press enter to stop");
            Console.ReadKey();
            nancyHost.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
