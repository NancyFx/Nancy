using System.Web;
using Spark;

namespace Nancy.ViewEngines.Spark.Caching
{
    public interface ICacheServiceProvider
    {
        ICacheService GetCacheService(HttpContextBase httpContext);
    }
}