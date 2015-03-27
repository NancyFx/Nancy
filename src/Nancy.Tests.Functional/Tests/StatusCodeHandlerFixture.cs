
namespace Nancy.Tests.Functional.Tests
{
    using System;

    using Nancy.Bootstrapper;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class StatusCodeHandlerFixture
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public StatusCodeHandlerFixture()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                configuration =>
                {
                    configuration.ApplicationStartup((container, pipelines) =>
                    {
                        pipelines.BeforeRequest += (ctx) =>
                        {
                            ctx.Items.Add("OnErrorException", new Exception("What a mistaka da maka!"));
                            throw new Exception("What a mistaka da maka!");
                        };
                    });

                    configuration.Module<PerRouteAuthModule>();
                    configuration.StatusCodeHandler<ErrorStatusCodeHandler>();
                });

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_Return_Error_Model_From_StatusCodeHandler_Before_Module_Built()
        {
            //Given
            var response = browser.Get("/nonsecured", with =>
            {
                with.HttpRequest();
                with.Accept("application/json");
            });

            //When
            var actualModel = response.Body.DeserializeJson<ErrorPageViewModel>();

            //Then
            Assert.Equal("Sorry, something went wrong", actualModel.Title);
            Assert.Equal("What a mistaka da maka!", actualModel.Summary);
        }
    }

    public class ErrorPageViewModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
    }

    public class ErrorStatusCodeHandler : IStatusCodeHandler
    {
        private readonly IResponseNegotiator responseNegotiator;

        public ErrorStatusCodeHandler(IResponseNegotiator responseNegotiator)
        {
            this.responseNegotiator = responseNegotiator;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = new Negotiator(context);

            var exception = context.Items["OnErrorException"] as Exception;

            response.WithModel(new ErrorPageViewModel
            {
                Title = "Sorry, something went wrong",
                Summary = exception.Message,
            }).WithStatusCode(statusCode).WithView("Error");

            var errorresponse = responseNegotiator.NegotiateResponse(response, context);
            context.Response = errorresponse;
        }
    }
}
