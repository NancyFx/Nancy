using System;

namespace Nancy.Bootstrapper
{
    public class VersionHeader
    {
        const string PipelineItemKey = "__VersionHeader";

        public void AddToPipeline(IPipelines pipelines)
        {
            var versionHeaderItem = new PipelineItem<Action<NancyContext>>(PipelineItemKey, context =>
                {
                    if (context.Response == null)
                    {
                        return;
                    }

                    var version = typeof(INancyEngine).Assembly.GetName().Version;
                    context.Response.Headers["Nancy-Version"] = version.ToString();
                });

            pipelines.AfterRequest += versionHeaderItem;
        }
    }
}