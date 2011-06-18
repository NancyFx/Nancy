namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Nancy.ViewEngines;

    public class FakeViewEngine : IViewEngine
    {
        public IEnumerable<string> Extensions { get; set; }

        public dynamic Model { get; set; }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            throw new NotImplementedException();
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            this.Model = model;
            return stream => { };
        }
    }
}