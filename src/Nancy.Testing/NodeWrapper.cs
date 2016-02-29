namespace Nancy.Testing
{
    using AngleSharp.Dom;

    /// <summary>
    /// Simple wrapper around a <see cref="IElement"/>.
    /// </summary>
    public class NodeWrapper
    {
        private readonly IElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeWrapper"/> class, for
        /// the provided <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The dom element that should be wrapped.</param>
        public NodeWrapper(IElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// Tests for the presence of an attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute to test for.</param>
        /// <returns>True if the node contains an attribute with the specified name, false otherwise.</returns>
        public bool HasAttribute(string name)
        {
            return this.element.HasAttribute(name);
        }

        /// <summary>
        /// Gets the attributes of the element
        /// </summary>
        public IndexHelper<string, string> Attributes
        {
            get
            {
                return new IndexHelper<string, string>(x =>
                {
                    var attribute = this.element.Attributes[x];

                    return (attribute != null)
                        ? attribute.Value
                        : null;
                });
            }
        }

        /// <summary>
        /// Gets the inner text of the node.
        /// </summary>
        /// <value>A <see cref="string"/> containing the inner text of the node.</value>
        public string InnerText
        {
            get { return this.element.TextContent; }
        }
    }
}
