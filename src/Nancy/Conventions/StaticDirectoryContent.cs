using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Conventions
{
    public class StaticDirectoryContent
    {
        NancyConventions conventions;

        public StaticDirectoryContent(NancyConventions conventions)
        {
            this.conventions = conventions;
        }

        public string this[string requestDirectory]
        {
            set
            {
                conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(requestDirectory, value));
            }
        }
    }
}
