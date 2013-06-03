namespace Nancy.ViewEngines.Spark
{
    public class NancyViewData
    {
        private readonly NancySparkView view;

        public NancyViewData(NancySparkView view)
        {
            this.view = view;
        }

        public object Eval(string key)
        {
            object value;
            return TryGetViewData(key, out value) ? value : null;
        }

        private bool TryGetViewData(string key, out object value)
        {
            if (this.view.ViewBag.ContainsKey(key) && this.view.ViewBag[key].HasValue)
            {
                value = this.view.ViewBag[key].Value;
                return true;
            }

            value = null;
            return false;
        }
    }
}