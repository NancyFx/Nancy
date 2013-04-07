using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Conventions
{
    public class StaticFileContent
    {
        NancyConventions conventions;

        public StaticFileContent(NancyConventions conventions)
        {
            this.conventions = conventions;
        }

        public string this[string requestFile]
        {
            set
            {
                conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddFile(requestFile, value));
            }
        }
    }
}
