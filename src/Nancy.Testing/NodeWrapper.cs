namespace Nancy.Testing
{
    using HtmlAgilityPack;

    /// <summary>
    /// Simple wrapper around HtmlNode
    /// </summary>
    public class NodeWrapper
    {
        private HtmlNode node;

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

        public string InnerText
        {
            get
            {
                return node.InnerText;
            }
        }

        public static implicit operator NodeWrapper(HtmlNode node)
        {
            return new NodeWrapper(node);
        }

        public static implicit operator HtmlNode(NodeWrapper wrapper)
        {
            return wrapper.node;
        }
    }
}