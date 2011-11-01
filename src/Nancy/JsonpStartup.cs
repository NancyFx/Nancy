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
            var item = new PipelineItem<Action<NancyContext>>("JSONP", context =>
            {
                bool isJson = context.Response.ContentType == "application/json";
                bool hasCallback = context.Request.Query["callback"].HasValue;

                if (isJson && hasCallback)
                {
                    // grab original contents for running later
                    Action<Stream> original = context.Response.Contents;
                    string callback = context.Request.Query["callback"].Value;

                    // set content type to application/javascript so browsers can handle it by default
                    // http://stackoverflow.com/questions/111302/best-content-type-to-serve-jsonp
                    context.Response.ContentType = "application/javascript";
                    context.Response.Contents = stream =>
                    {
                        // disposing of stream is handled elsewhere
                        StreamWriter writer = new StreamWriter(stream)
                        {
                            AutoFlush = true
                        };

                        writer.Write("{0}(", callback);
                        original(stream);
                        writer.Write(");");
                    };
                }
            });

            pipelines.AfterRequest.AddItemToEndOfPipeline(item);
        }
    }
}
