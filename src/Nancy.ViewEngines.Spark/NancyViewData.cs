namespace Nancy.ViewEngines.Spark
{
    public class NancyViewData
    {
        private readonly NancySparkView _view;

        public NancyViewData(NancySparkView view)
        {
            _view = view;
        }

        public object Eval(string key)
        {
            object value;
            return TryGetViewData(key, out value) ? value : null;
        }

        private bool TryGetViewData(string key, out object value)
        {
            if (_view.ViewBag.ContainsKey(key) && _view.ViewBag[key].HasValue)
            {
                value = _view.ViewBag[key].Value;
                return true;
            }

            value = null;
            return false;
        }
    }
}