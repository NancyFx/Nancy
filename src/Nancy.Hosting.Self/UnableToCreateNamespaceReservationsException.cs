namespace Nancy.Hosting.Self
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class UnableToCreateNamespaceReservationsException : Exception
    {
        private readonly IEnumerable<string> prefixes;
        private readonly string user;

        public UnableToCreateNamespaceReservationsException(IEnumerable<string> prefixes, string user)
        {
            this.prefixes = prefixes;
            this.user = user;
        }

        public override string Message
        {
            get
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("The Nancy self host was unable to start, as no namespace reservation existed for the provided url(s).");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine("Please either enable CreateNamespaceReservations on the HostConfiguration provided to the NancyHost,");
                stringBuilder.AppendLine("or create the reservations manually with the (elevated) command(s):");
                stringBuilder.AppendLine();

                foreach (var prefix in prefixes)
                {
                    var command = NetSh.GetNetShAddUrlAclCommand(prefix, user);
                    stringBuilder.AppendLine(command.ToString());
                }

                return stringBuilder.ToString();
            }
        }
    }
}