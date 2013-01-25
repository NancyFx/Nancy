namespace Nancy.Testing
{
    using CsQuery.Implementation;

    /// <summary>
    /// Simple wrapper around a <see cref="DomElement"/>.
    /// </summary>
    public class NodeWrapper
    {
        private readonly DomElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeWrapper"/> class, for
        /// the provided <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The dom element that should be wrapped.</param>
        public NodeWrapper(DomElement element)
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

        public IndexHelper<string, string> Attributes
        {
            get { return new IndexHelper<string, string>(x => this.element.Attributes[x]); }
        }

        /// <summary>
        /// Gets the inner text of the node.
        /// </summary>
        /// <value>A <see cref="string"/> containing the inner text of the node.</value>
        public string InnerText
        {
            get { return this.element.InnerText; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DomElement"/> to <see cref="Nancy.Testing.NodeWrapper"/>.
        /// </summary>
        /// <param name="node">The <see cref="DomElement"/> that should be cast.</param>
        /// <returns>An <see cref="NodeWrapper"/> instance, that contains the results of the cast.</returns>
        public static implicit operator NodeWrapper(DomElement node)
        {
            return new NodeWrapper(node);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nancy.Testing.NodeWrapper"/> to <see cref="DomElement"/>.
        /// </summary>
        /// <param name="wrapper">The <see cref="NodeWrapper"/> that should be cast.</param>
        /// <returns>A <see cref="DomElement"/> instance, that contains the results of the cast.</returns>
        public static implicit operator DomElement(NodeWrapper wrapper)
        {
            return wrapper.element;
        }
    }
}