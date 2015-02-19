namespace Nancy.ViewEngines.Spark
{
    using System.IO;
    using System.Web;

    using global::Spark;

    public abstract class NancySparkView : SparkViewBase
    {
        private readonly NancyViewData viewData;

        protected NancySparkView()
        {
            this.viewData = new NancyViewData(this);
        }

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

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                if (this.RenderContext == null)
                {
                    return null;
                }

                return this.RenderContext.Context == null ? null : this.RenderContext.Context.ViewBag;
            }
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

        /// <summary>
        /// Non-model specific data retrieved using the &lt;viewdata /&gt; tag in Spark views
        /// </summary>
        /// <remarks>See more on http://sparkviewengine.com/documentation/variables#Usingviewdata</remarks>
        public NancyViewData ViewData
        {
            get { return this.viewData; }
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