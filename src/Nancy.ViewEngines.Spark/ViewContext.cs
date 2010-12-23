namespace Nancy.ViewEngines.Spark
{
    using global::Spark;

    public class ViewContext : ActionContext
    {
        public ViewContext(ActionContext actionContext, ISparkView view) :
            base(actionContext.HttpContext, actionContext.ViewPath)
        {
            View = view;
        }

        public ISparkView View { get; set; }
    }
}