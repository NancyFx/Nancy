using System.Web;
using Spark;
using Spark.Caching;

namespace Nancy.ViewEngines.Spark.Caching
{
    public class DefaultCacheServiceProvider : ICacheServiceProvider
    {
        #region ICacheServiceProvider Members

        public ICacheService GetCacheService(HttpContextBase httpContext)
        {
            if (httpContext != null && httpContext.Cache != null)
                return new DefaultCacheService(httpContext.Cache);
            return null;
        }

        #endregion
    }
}