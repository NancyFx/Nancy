namespace Nancy.ViewEngines.Razor
{
    using System.Dynamic;

    /// <summary>
    /// Returns text from an implemented ITextResource
    /// </summary>
    public class TextResourceFinder : DynamicObject
    {
        private readonly ITextResource textResource;
        private readonly NancyContext context;

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
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.textResource[binder.Name, this.context];
            return true;
        }
    }
}
