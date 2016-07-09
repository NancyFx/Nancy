namespace Nancy.Tests.Functional.Modules
{
    using System;

    public class ThrowingModule : NancyModule
    {
        public ThrowingModule()
        {
            Get("/", args =>
            {
                throw new InvalidOperationException("Oh noes!");
                return 500;
            });

            this.OnError.AddItemToEndOfPipeline((context, ex) => new Error(ex.Message));
        }

        public class Error
        {
            public Error() : this(string.Empty)
            {
            }

            public Error(string message)
            {
                this.Message = message;
            }

            public string Message { get; set; }
        }
    }
}