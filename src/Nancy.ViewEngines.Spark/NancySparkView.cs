namespace Nancy.ViewEngines.Spark
{
    using System.IO;
    using System.Web;
    using global::Spark;

    public abstract class NancySparkView : SparkViewBase
    {
        public object Model { get; set; }

        public ViewContext ViewContext { get; set; }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            base.RenderView(Writer);
        }

        public string H(object value)
        {
            return HttpUtility.HtmlEncode(value.ToString());
        }

        public object HTML(object value)
        {
            return value;
        }
    }

    public abstract class NancySparkView<TModel> : NancySparkView
    {
        public new TModel Model { get; private set; }

        public void SetModel(object model)
        {
            Model = (model is TModel) ? (TModel)model : default(TModel);
        }
    }
}