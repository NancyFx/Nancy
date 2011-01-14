using System;

namespace Nancy.Demo
{
    public class ApplicationDependencyClass : IApplicationDependency
    {
        private readonly DateTime _CurrentDateTime;

        /// <summary>
        /// Initializes a new instance of the RequestDependencyClass class.
        /// </summary>
        public ApplicationDependencyClass()
        {
            _CurrentDateTime = DateTime.Now;
        }

        public string GetContent()
        {
            return "This is an application level dependency, constructed on: " + _CurrentDateTime.ToLongTimeString();
        }
    }
}
