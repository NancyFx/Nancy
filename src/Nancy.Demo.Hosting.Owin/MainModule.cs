namespace Nancy.Demo.Hosting.Owin
{
    using System.Threading;
    using System.Threading.Tasks;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["", true] = Root;
        }

        private Task<object> Root(dynamic o, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(View["Root"]);
        }
    }
}