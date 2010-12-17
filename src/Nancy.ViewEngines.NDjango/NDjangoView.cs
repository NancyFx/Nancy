namespace Nancy.ViewEngines.NDjango
{
    using System.Collections.Generic;
    using System.IO;
    using global::NDjango.Interfaces;

    public class NDjangoView : IView
    {
        private readonly ITemplate template;
        private readonly ITemplateManager templateManager;

        public NDjangoView(ITemplate template, ITemplateManager templateManager)
        {
            this.template = template;
            this.templateManager = templateManager;
        }

        public string Code { get; set; }

        public object Model { get; set; }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            var context = new Dictionary<string, object> {{"Model", Model}};
            var reader = template.Walk(templateManager, context);

            var buffer = new char[4096];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                Writer.Write(buffer, 0, count);
            }

            Writer.Flush();
        }
    }
}