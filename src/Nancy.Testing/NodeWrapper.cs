namespace Nancy.Testing
{
    using HtmlAgilityPack;

    /// <summary>
    /// Simple wrapper around a <see cref="HtmlNode"/>.
    /// </summary>
    public class NodeWrapper
    {
        private readonly HtmlNode node;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeWrapper"/> class.
        /// </summary>
        /// <param name="node">The <see cref="HtmlNode"/> that should be wrapped.</param>
        private NodeWrapper(HtmlNode node)
        {
            this.node = node;
        }

        public IndexHelper<string, string> Attribute
        {
            get
            {
                return new IndexHelper<string, string>(s => node.Attributes[s].Value);
            }
        }

        /// <summary>
        /// Gets the inner text of the node.
        /// </summary>
        /// <value>A <see cref="string"/> containing the inner text of the node.</value>
        public string InnerText
        {
            get
            {
                return node.InnerText;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HtmlAgilityPack.HtmlNode"/> to <see cref="Nancy.Testing.NodeWrapper"/>.
        /// </summary>
        /// <param name="node">The <see cref="HtmlNode"/> that should be cast.</param>
        /// <returns>An <see cref="NodeWrapper"/> instance, that contains the results of the cast.</returns>
        public static implicit operator NodeWrapper(HtmlNode node)
        {
            return new NodeWrapper(node);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nancy.Testing.NodeWrapper"/> to <see cref="HtmlAgilityPack.HtmlNode"/>.
        /// </summary>
        /// <param name="wrapper">The <see cref="NodeWrapper"/> that should be cast.</param>
        /// <returns>A <see cref="HtmlNode"/> instance, that contains the results of the cast.</returns>
        public static implicit operator HtmlNode(NodeWrapper wrapper)
        {
            return wrapper.node;
        }
    }
}