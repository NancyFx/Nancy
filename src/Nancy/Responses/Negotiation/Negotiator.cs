namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Request response content negotiator.
    /// </summary>
    /// <seealso cref="Nancy.IHideObjectMembers" />
    public class Negotiator : IHideObjectMembers
    {
        // TODO - this perhaps should be an interface, along with the view thing above
        // that would then wrap this to give more granular extension point for things like
        // AsNegotiated

        /// <summary>
        /// Initializes a new instance of the <see cref="Negotiator"/> class,
        /// with the provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that should be negotiated.</param>
        public Negotiator(NancyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.NegotiationContext = context.NegotiationContext;
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<Negotiator> GetAwaiter()
        {
            return Task.FromResult(this).GetAwaiter();
        }

        /// <summary>
        /// Gets the <see cref="NegotiationContext"/> used by the negotiator.
        /// </summary>
        /// <value>A <see cref="NegotiationContext"/> instance.</value>
        public NegotiationContext NegotiationContext { get; private set; }
    }
}