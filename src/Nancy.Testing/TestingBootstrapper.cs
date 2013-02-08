using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Testing
{
    public class TestingBootstrapper : ConfigurableBootstrapper
    {
        // Hotfix for issue #624
        public TestingBootstrapper() : base(x => x.RootPathProvider(new DefaultRootPathProvider()))
        {
            
        }
    }
}
