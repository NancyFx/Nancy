namespace Nancy.ViewEngines.Spark
{
    using System.Web;
    using global::Spark;

    public abstract class NancySparkView : SparkViewBase
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

        public object Model { get; set; }
    }

    public abstract class NancySparkView<TModel> : NancySparkView
    {
        public new TModel Model { get; private set; }

        public void SetModel(object model)
        {
            this.Model = (model is TModel) ? (TModel)model : default(TModel);
        }
    }
}