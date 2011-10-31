namespace Nancy.ViewEngines.Spark
{
    using System.IO;
    using System.Web;
    using global::Spark;

    public abstract class NancySparkView : SparkViewBase
    {
        public object Model { get; set; }

        public TextWriter Writer { get; set; }

        public IRenderContext RenderContext { get; set; }

        public void Execute()
        {
            base.RenderView(Writer);
        }

        public string H(object value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        public object HTML(object value)
        {
            return value;
        }

        public string AntiForgeryToken()
        {
            var tokenKeyValue = this.RenderContext.GetCsrfToken();

            return string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", tokenKeyValue.Key, tokenKeyValue.Value);
        }

        public virtual void SetModel(object model)
        {
            this.Model = model;
        }

        public string SiteResource(string path)
        {
            return this.RenderContext.ParsePath(path);
        }
    }

    public abstract class NancySparkView<TModel> : NancySparkView
    {
        public new TModel Model { get; private set; }

        public override void SetModel(object model)
        {
            Model = (model is TModel) ? (TModel)model : default(TModel);
        }
    }
}