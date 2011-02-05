namespace Nancy.Demo
{
    using System;

    public class RequestDependencyClass : RequestDependency
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