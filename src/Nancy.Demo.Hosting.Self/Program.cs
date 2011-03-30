namespace Nancy.Demo.Hosting.Self
{
    using System;
    using Nancy.Hosting.Self;

    class Program
    {
        static void Main()
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