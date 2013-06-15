namespace Nancy.Hosting.Self
{
    using System;

    internal static class NetSh
    {
        public static bool AddUrlAcl(string url, string user)
        {
            try
            {
                Console.WriteLine("Trying to add a namespace reservation");
                var command = GetNetShAddUrlAclCommand(url, user);
                return Uac.RunElevated(command.File, command.Args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to add reservation.");
                Console.WriteLine(e);
                return false;
            }
        }

        public static NetShCommand GetNetShAddUrlAclCommand(string url, string user)
        {
            var args = string.Format("http add urlacl url={0} user={1}", url, user);
            return new NetShCommand("netsh", args);
        }
    }
}