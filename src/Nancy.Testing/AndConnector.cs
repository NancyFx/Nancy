namespace Nancy.Testing
{
    public class AndConnector<TSource> : IHideObjectMembers
    {
        private TSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndConnector{TSource}"/> class.
        /// </summary>
        /// <param name="source">
        /// Source object
        /// </param>
        public AndConnector(TSource source)
        {
            this.source = source;
        }

        /// <summary>
        /// And
        /// </summary>
        public TSource And
        {
            get
            {
                return this.source;
            }
        }
    }
}