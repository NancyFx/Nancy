namespace Nancy.ViewEngines.Spark
{
    using System.IO;
    using System.Web;
    using global::Spark;

    public abstract class SparkView : SparkViewBase, IView
    {
        public ViewContext ViewContext { get; set; }

        public string H(object value)
        {
            return HttpUtility.HtmlEncode(value.ToString());
        }

        public object HTML(object value)
        {
            return value;
        }

        public string Code { get; set; }

        public object Model { get; set; }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            RenderView(Writer);
        }
    }

    public abstract class SparkView<TModel> : SparkView
    {
        public new TModel Model { get; private set; }

        public void SetModel(object model)
        {
            Model = model is TModel ? (TModel)model : default(TModel);
        }
    }
}