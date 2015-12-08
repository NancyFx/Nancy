namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;

    using Nancy.Responses;
    using Nancy.ViewEngines;

    public class FakeViewEngine : IViewEngine
    {
        public FakeViewEngine()
        {
            this.Extensions = new[] { "html " };
        }

        public IEnumerable<string> Extensions { get; set; }

        public dynamic Model { get; set; }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            this.Model = model;
            return new HtmlResponse();
        }
    }
}