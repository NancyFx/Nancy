namespace Nancy.Hosting.Self
{
    using System;
    using System.Diagnostics;

    internal class NetSh
    {
        public static bool AddUrlAcl(string url, string user)
        {
            try
            {
                Console.WriteLine("Trying to add a namespace reservation");
                return RunNetShElevated(url, user);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to add reservation.");
                Console.WriteLine(e);
                return false;
            }
        }

        private static bool RunNetShElevated(string url, string user)
        {
            var args = string.Format("http add urlacl url={0} user={1}", url, user);
            Console.WriteLine("netsh " + args);

            var process = CreateProcess(args);
            process.Start();
            process.WaitForExit();

            var exitCode = process.ExitCode;
            if (exitCode == 0)
            {
                return true;
            }

            Console.WriteLine("Failed to add reservation. Exit code: {0}", exitCode);
            return false;
        }

        private static Process CreateProcess(string args)
        {
            return new Process 
            {
                StartInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    Arguments = args,
                    FileName = "netsh",
                }
            };
        }
    }
}