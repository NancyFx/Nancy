namespace Nancy
{
    using System;

    public interface IRoute
    {
        Func<object, Response> Action { get; set; }

        Response Invoke();
    }

    public class Route : IRoute
    {
        public Func<object, Response> Action { get; set; }

        public Response Invoke()
        {
            throw new NotImplementedException();
        }
    }
}