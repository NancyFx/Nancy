namespace Nancy.Demo
{
    using System;

    public class ApplicationDependencyClass : IApplicationDependency
    {
        private readonly DateTime currentDateTime;

        /// <summary>
        /// Initializes a new instance of the RequestDependencyClass class.
        /// </summary>
        public ApplicationDependencyClass()
        {
            this.currentDateTime = DateTime.Now;
        }

        public string GetContent()
        {
            return "This is an application level dependency, constructed on: " + this.currentDateTime.ToLongTimeString();
        }
    }
}