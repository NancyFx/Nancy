namespace Nancy.Demo.Hosting.Self
{
    using System;
    using System.Diagnostics;

    using Nancy.Hosting.Self;

    class Program
    {
        static void Main()
        {
            using (var nancyHost = new NancyHost(new Uri("http://localhost:8888/nancy/"), new Uri("http://127.0.0.1:8898/nancy/"), new Uri("http://localhost:8889/nancytoo/")))
            {
                nancyHost.Start();

                Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/nancy/. Press enter to stop");
                try
                {
                    Process.Start("http://localhost:8888/nancy/");
                }
                catch (Exception)
                {
                }
                Console.ReadKey();
            }

            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
