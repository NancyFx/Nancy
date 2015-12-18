namespace Nancy.Demo.Bootstrapping.Aspnet
{
    using System;

    /// <summary>
    /// A module dependency that will have a per-request lifetime scope.
    /// </summary>
    public class RequestDependencyClass : IRequestDependency
    {
        private readonly DateTime currentDateTime;

        /// <summary>
        /// Initializes a new instance of the RequestDependencyClass class.
        /// </summary>
        public RequestDependencyClass()
        {
            this.currentDateTime = DateTime.Now;
        }

        public string GetContent()
		{
            return "This is a per-request dependency, constructed on: " + this.currentDateTime.ToLongTimeString();
		}
    }
}