using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Nancy.Tests.Fakes;
using Nancy.ViewEngines;

namespace Nancy.Tests.Unit
{
    public class DefaultTemplateEngineSelectorFixture
    {
        private DefaultTemplateEngineSelector _TemplateEngineSelector;

        /// <summary>
        /// Initializes a new instance of the DefaultTemplateEngineSelectorFixture class.
        /// </summary>
        public DefaultTemplateEngineSelectorFixture()
        {
            _TemplateEngineSelector = new DefaultTemplateEngineSelector(new IViewEngineRegistry[] {new FakeViewEngineRegistry()});
        }

        [Fact]
        public void Should_return_null_for_an_unknown_view_extension()
        {
            _TemplateEngineSelector.GetTemplateProcessor(".unknown").ShouldBeNull();
        }

        [Fact]
        public void Should_return_the_processor_for_a_given_extension()
        {
            _TemplateEngineSelector.GetTemplateProcessor(".leto2").ShouldBeSameAs(FakeViewEngineRegistry.Executor);
        }

        [Fact]
        public void Should_be_case_intensitive_about_view_extensions()
        {
            _TemplateEngineSelector.GetTemplateProcessor(".LetO2").ShouldBeSameAs(FakeViewEngineRegistry.Executor);
        }
    }
}
