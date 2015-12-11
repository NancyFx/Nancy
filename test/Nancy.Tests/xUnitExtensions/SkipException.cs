namespace Nancy.Tests.xUnitExtensions
{
    using System;

    public class SkipException : Exception
    {
        public SkipException(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; set; }
    }
}
