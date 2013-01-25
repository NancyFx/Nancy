namespace Nancy.Demo.CustomModule
{
    using System;

    public class NancyRouteAttribute : Attribute
    {
        /// <summary>
        /// The method for the route
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The path for the route
        /// </summary>
        public string Path { get; set; }

        public NancyRouteAttribute(string method, string path)
        {
            this.Method = method;
            this.Path = path;
        }
    }
}