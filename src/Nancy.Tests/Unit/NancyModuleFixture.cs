namespace Nancy.Tests.Unit
{
    using System;
    using System.IO;
    using FakeItEasy;
    using Fakes;
    using Xunit;

    public class NancyModuleFixture
    {
        [Fact]
        public void Should_execute_the_default_processor_unregistered_extension()
        {
            var application = A.Fake<INancyApplication>();
            var module = new FakeNancyModuleWithoutBasePath {Application = application};
            var action = new Action<Stream>((s) => { });
            var processor = new Func<string, object, Action<Stream>>((a, b) => action);

            A.CallTo(() => application.GetTemplateProcessor(".txt")).Returns(null);
            A.CallTo(() => application.DefaultProcessor).Returns(processor);

            module.SmartView("file.txt").ShouldBeSameAs(action);
        }

        [Fact]
        public void Should_execute_the_processor_associated_with_the_extension()
        {
            var application = A.Fake<INancyApplication>();
            var module = new FakeNancyModuleWithoutBasePath { Application = application };
            var action = new Action<Stream>((s) => { });
            var processor = new Func<string, object, Action<Stream>>((a, b) => action);

            A.CallTo(() => application.GetTemplateProcessor(".razor")).Returns(processor);            

            module.SmartView("file2.razor").ShouldBeSameAs(action);
        }
    }
}