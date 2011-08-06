namespace Nancy.Security
{
    using System;

    public sealed class CsrfToken
    {
        public byte[] RandomBytes { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Username { get; set; }

        public string Salt { get; set; }
    }
}