using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nancy.ViewEngines.Razor
{
    internal class SilentDiagnosticSource : System.Diagnostics.DiagnosticSource
    {
        public override void Write(string name, object value)
        {
            // Do nothing
        }

        public override bool IsEnabled(string name) => true;
    }
}
