namespace Nancy.Routing
{
    using System;

    public interface IRoute
    {
        Func<object, Response> Action { get; set; }

        string Path { get; }

        dynamic Parameters { get; }

        Response Invoke();
    }
}