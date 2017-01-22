namespace Nancy
{
    /// <summary>
    /// Defines a pipeline item
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    public class PipelineItem<TDelegate>
    {
        /// <summary>
        /// Gets or sets the pipeline item name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public TDelegate Delegate { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineItem{TDelegate}"/> class, with
        /// the provided <paramref name="name"/> and <paramref name="delegate"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="delegate">The delegate.</param>
        public PipelineItem(string name, TDelegate @delegate)
        {
            this.Name = name;
            this.Delegate = @delegate;
        }


        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="TDelegate"/> cref="TDelegate"/> to <see cref="PipelineItem{TDelegate}"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator PipelineItem<TDelegate>(TDelegate action)
        {
            return new PipelineItem<TDelegate>(null, action);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="PipelineItem{TDelegate}"/> to <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator TDelegate(PipelineItem<TDelegate> pipelineItem)
        {
            return pipelineItem.Delegate;
        }
    }
}