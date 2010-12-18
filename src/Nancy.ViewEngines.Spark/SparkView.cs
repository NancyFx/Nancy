using System.Web;
using Spark;

namespace Nancy.ViewEngines.Spark
{
    public abstract class SparkView : SparkViewBase
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
    }

    public abstract class SparkView<TModel> : SparkView where TModel : class
    {
        public TModel Model { get; private set; }

        public void SetModel(object model)
        {
            Model = model as TModel;
        }
    }
}