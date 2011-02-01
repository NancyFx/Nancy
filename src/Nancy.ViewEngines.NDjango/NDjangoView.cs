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

        public object Model { get; set; }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            var context = new Dictionary<string, object> {{"Model", Model}};
            var reader = template.Walk(templateManager, context);

            Writer.Write(reader.ReadToEnd());
        }
    }
}