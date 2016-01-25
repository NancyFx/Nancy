namespace Nancy.Tests.Functional.Modules
{
    using System;

    public class ThrowingModule : LegacyNancyModule
    {
        public ThrowingModule()
        {
            this.Get["/"] = _ =>
            {
                throw new InvalidOperationException("Oh noes!");
            };

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