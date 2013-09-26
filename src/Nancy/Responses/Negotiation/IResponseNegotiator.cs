namespace Nancy.Responses.Negotiation
{
    public interface IResponseNegotiator 
    {
        Response NegotiateResponse(dynamic result, NancyContext context);
    }
}