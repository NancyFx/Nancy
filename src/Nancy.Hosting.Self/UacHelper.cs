namespace Nancy.Hosting.Self
{
    using System.Diagnostics;

    /// <summary>
    /// Helpers for UAC
    /// </summary>
    public static class UacHelper
    {
        /// <summary>
        /// Run an executable elevated
        /// </summary>
        /// <param name="file">File to execute</param>
        /// <param name="args">Arguments to pass to the executable</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RunElevated(string file, string args)
        {
            var process = CreateProcess(args, file);

            process.Start();
            process.WaitForExit();

            return process.ExitCode == 0;
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