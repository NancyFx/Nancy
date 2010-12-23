namespace Nancy.ViewEngines.Spark.Caching
{
    using System.Web;
    using global::Spark;
    using global::Spark.Caching;

    public class DefaultCacheServiceProvider : ICacheServiceProvider
    {
        public ICacheService GetCacheService(HttpContextBase httpContext)
        {
            if (httpContext != null && httpContext.Cache != null)
            {
                return new DefaultCacheService(httpContext.Cache);
            }
                
            return null;
        }
    }
}