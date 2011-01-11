using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.BootStrapper
{
    /// <summary>
    /// BootStrapper for the Nancy Engine
    /// </summary>
    public interface INancyBootStrapper
    {
        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        INancyEngine GetEngine();
    }
}
