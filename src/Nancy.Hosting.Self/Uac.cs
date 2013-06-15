namespace Nancy.Hosting.Self
{
    using System;
    using System.Diagnostics;

    internal static class Uac
    {
        public static bool RunElevated(string file, string args)
        {
            Console.WriteLine("{0} {1}", file, args);

            var process = CreateProcess(args, file);
            process.Start();
            process.WaitForExit();

            var exitCode = process.ExitCode;
            if (exitCode == 0)
            {
                return true;
            }

            Console.WriteLine("Failed to execute command. Exit code: {0}", exitCode);
            return false;
        }

        private static Process CreateProcess(string args, string file)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    Arguments = args,
                    FileName = file,
                }
            };
        }
    }
}