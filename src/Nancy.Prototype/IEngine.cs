namespace Nancy.Prototype
{
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Prototype.Http;

    public interface IEngine
    {
        Task HandleRequest(HttpContext context, CancellationToken cancellationToken);
    }
}
