using System;

namespace Nancy.Demo
{
    public class RequestDependencyClass : IRequestDependency
    {
        private readonly DateTime _CurrentDateTime;

        /// <summary>
        /// Initializes a new instance of the RequestDependencyClass class.
        /// </summary>
        public RequestDependencyClass()
        {
            _CurrentDateTime = DateTime.Now;
        }

        public string GetContent()
		{
            return "This is a per-request dependency, constructed on: " + _CurrentDateTime.ToLongTimeString();
		}
    }
}
