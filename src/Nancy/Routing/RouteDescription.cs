namespace Nancy.Routing
{
    using System;

    public sealed class RouteDescription
    {
<<<<<<< HEAD
        public RouteDescription(string method, string path, Func<Request, bool> condition)
=======
        public RouteDescription(string method, string path, Func<NancyContext, bool> condition)
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
        {
            if (String.IsNullOrEmpty(method))
            {
                throw new ArgumentException("Method must be specified", method);
            }

            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path must be specified", method);
            }

            this.Method = method;
            this.Path = path;
            this.Condition = condition;
        }

<<<<<<< HEAD
        public Func<Request, bool> Condition { get; private set; }
=======
        public Func<NancyContext, bool> Condition { get; private set; }
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147

        public string Method { get; private set; }

        public string Path { get; private set; }
    }
}