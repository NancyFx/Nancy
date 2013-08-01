namespace Nancy.Demo.Async
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.Bootstrapper;

    class Program
    {
        static void Main(string[] args)
        {
            ShowHeader();

            var engine = GetEngine();
            var request = GetRequest();

            var tcs = new TaskCompletionSource<NancyContext>(); 
            var completionTask = tcs.Task;
            
            Console.Write("Running: ");

            engine.HandleRequest(request, tcs.SetResult, tcs.SetException);

            while (!completionTask.IsCompleted && !completionTask.IsFaulted)
            {
                Console.Write("*");
                Thread.Sleep(100);
            }

            string result = "Unknown";
            if (completionTask.IsFaulted && completionTask.Exception != null)
            {
                result = completionTask.Exception.GetType().ToString();
            }

            if (completionTask.IsCompleted && completionTask.Result != null)
            {
                result = GetBody(completionTask.Result);
            }

            Console.WriteLine("\nResult: \n\n{0}", result);
            Console.WriteLine("\nPress any key to close.");
            Console.ReadKey();
        }

        private static INancyEngine GetEngine()
        {
            var bootstrapper = NancyBootstrapperLocator.Bootstrapper;
            bootstrapper.Initialise();

            var engine = bootstrapper.GetEngine();
            return engine;
        }

        private static void ShowHeader()
        {
            Console.WriteLine("Async Demo");
            Console.WriteLine();
            Console.WriteLine("A long running async request will be executed and until it is complete");
            Console.WriteLine("a series of '*' characters should appear every 100 milliseconds. If this was");
            Console.WriteLine("executed syncronously then the main thread would block and no characters");
            Console.WriteLine("would appear.");
            Console.WriteLine();
        }

        private static string GetBody(NancyContext result)
        {
            string output;

            using (var memoryStream = new MemoryStream())
            {
                result.Response.Contents.Invoke(memoryStream);
                output = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }

            return output;
        }

        private static Request GetRequest(string path = "/")
        {
            return new Request("GET", path, "http");
        }
    }
}
