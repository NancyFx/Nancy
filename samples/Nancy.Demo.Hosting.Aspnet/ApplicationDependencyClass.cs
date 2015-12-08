namespace Nancy.Demo.Hosting.Aspnet
{
    using System;

    /// <summary>
    /// A module dependency that will have an application lifetime scope.
    /// </summary>
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