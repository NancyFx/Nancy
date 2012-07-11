namespace Nancy.Responses.Negotiation
{
    public class Negotiator : IHideObjectMembers
    {
        // TODO - this perhaps should be an interface, along with the view thing above
        // that would then wrap this to give more granular extension point for things like
        // AsNegotiated
        public NegotiationContext NegotiationContext { get; private set; }

        public Negotiator(NancyContext context)
        {
            this.NegotiationContext = context.NegotiationContext;
        }
    }
}