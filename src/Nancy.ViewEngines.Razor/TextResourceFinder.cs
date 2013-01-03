namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Dynamic;
    using Nancy.Localization;

    /// <summary>
    /// Returns text from an implemented ITextResource
    /// </summary>
    public class TextResourceFinder : DynamicObject
    {
        private readonly ITextResource textResource;
        private readonly NancyContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextResourceFinder"/> class.
        /// </summary>
        /// <param name="textResource">The <see cref="ITextResource"/> that should be used by the TextResourceFinder</param>
        /// <param name="context">The <see cref="NancyContext"/> that should be used by the TextResourceFinder</param>
        public TextResourceFinder(ITextResource textResource, NancyContext context)
        {
            this.textResource = textResource;
            this.context = context;
        }

        /// <summary>
        /// Finds text resource
        /// </summary>
        /// <param name="binder">GetMemberBinder with dynamic text key</param>
        /// <param name="result">Text item</param>
        /// <returns>Returns a value or a non existing value from the <see cref="ITextResource"/> implementation</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = 
                new DynamicMemberChainer(binder.Name, this.context, this.textResource);

            return true;
        }

        public class DynamicMemberChainer : DynamicObject
        {
            private string memberName;
            private readonly NancyContext context;
            private readonly ITextResource textResource;

            public DynamicMemberChainer(string memberName, NancyContext context, ITextResource resource)
            {
                this.memberName = memberName;
                this.context = context;
                this.textResource = resource;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                this.memberName =
                    string.Concat(this.memberName, ".", binder.Name);

                result = this;

                return true;
            }

            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                if (binder.ReturnType == typeof(string))
                {
                    result = this.textResource[this.memberName, this.context];
                    return true;
                }

                throw new InvalidOperationException("Cannot cast dynamic member access to anything else than a string.");
            }

            public override string ToString()
            {
                return this.textResource[this.memberName, this.context]; ;
            }
        }
    }
}
