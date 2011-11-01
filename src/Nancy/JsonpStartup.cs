using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Bootstrapper;
using Nancy.Responses;
using System.IO;

namespace Nancy
{
    public class JsonpStartup : IStartup
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return null; }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return null; }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return null; }
        }

        public void Initialize(IPipelines pipelines)
        {
            Nancy.Jsonp.Enable(pipelines);
        }
    }
}
