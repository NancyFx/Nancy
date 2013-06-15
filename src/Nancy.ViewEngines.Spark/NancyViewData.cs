namespace Nancy.ViewEngines.Spark
{
    /// <summary>
    /// Wraps <see cref="NancySparkView.ViewBag"/> so that it can be retrieved in view by using Spark's &lt;viewdata /&gt; tag
    /// </summary>
    /// <example> In route add something to ViewBag:
    /// <code>
    /// this.ViewBag["foo"] = "bar";
    /// </code>
    /// In view:
    /// <code>
    /// &lt;viewdata foo="string" /&gt;
    /// 
    /// The value of foo is: ${foo}
    /// </code>
    /// </example>
    public class NancyViewData
    {
        private readonly NancySparkView view;

        /// <summary>
        /// Initializes a new instance of <see cref="NancyViewData"/>
        /// </summary>
        /// <param name="view">view, whose <see cref="NancySparkView.ViewBag"/> will be used to retrieve values</param>
        public NancyViewData(NancySparkView view)
        {
            this.view = view;
        }

        /// <summary>
        /// Gets a view data value from the <see cref="NancySparkView.ViewBag"/>
        /// </summary>
        /// <param name="key"><see cref="NancySparkView.ViewBag"/> key</param>
        /// <returns>null if the key wasn't present in the <see cref="NancySparkView.ViewBag"/></returns>
        /// <remarks>This method is output to the generated view class when &lt;viewdata /&gt; tag is used</remarks>
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