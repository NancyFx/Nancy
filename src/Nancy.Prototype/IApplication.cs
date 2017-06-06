namespace Nancy.Prototype
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Prototype.Http;

    public interface IApplication : IDisposable
    {
        Task HandleRequest(HttpContext context, CancellationToken cancellationToken);
    }

    public interface IApplication<out TContainer> : IApplication
    {
        TContainer Container { get; }
    }
}
