namespace Nancy.Bootstrapper
{
    public interface IRequestPipelinesFactory
    {
        IPipelines CreateRequestPipeline(NancyContext context);
    }
}