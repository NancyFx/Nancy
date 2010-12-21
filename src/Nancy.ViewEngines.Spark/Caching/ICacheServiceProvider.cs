namespace Nancy.ViewEngines.Spark.Caching
{
    using System.Web;
    using global::Spark;

    public interface ICacheServiceProvider
    {
        ICacheService GetCacheService(HttpContextBase httpContext);
    }
}