using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Tests.xUnitExtensions
{
    public class SkipException : Exception
    {
        public SkipException(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; set; }
    }
}
