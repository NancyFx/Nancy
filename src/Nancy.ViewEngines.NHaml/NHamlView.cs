namespace Nancy.ViewEngines.NHaml
{
    using System.IO;
    using global::NHaml;

    public class NHamlView<TModel> : Template, IView
    {
        public string Code { get; set; }

        object IView.Model
        {
            get { return Model; }
            set { Model = (TModel) value; }
        }

        public TModel Model { get; set; }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            Render(Writer);
        }
    }
}